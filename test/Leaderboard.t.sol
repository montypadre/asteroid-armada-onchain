// SPDX-License-Identifier: SEE LICENSE IN LICENSE
pragma solidity ^0.8.24;

import {Test} from "forge-std/Test.sol";
import {Leaderboard} from "../src/Leaderboard.sol";
import {Ownable} from "@openzeppelin/contracts/access/Ownable.sol";

contract LeaderboardTest is Test {
    Leaderboard leaderboard;
    address stranger = makeAddr("stranger");

    function setUp() public {
        leaderboard = new Leaderboard();
    }

    // ---- Happy path ----

    function test_AddScore_InsertsIntoEmptyBoard() public {
        bool inserted = leaderboard.addScore("alice", 100);
        assertTrue(inserted);

        Leaderboard.Entry[5] memory board = leaderboard.getLeaderboard();
        assertEq(board[0].player, "alice");
        assertEq(board[0].score, 100);
    }

    function test_AddScore_EmitsScoreAddedEvent() public {
        vm.expectEmit(true, true, true, true);
        emit ScoreAdded("bob", 50, 0);
        leaderboard.addScore("bob", 50);
    }

    function test_AddScore_MaintainsDescendingOrder() public {
        leaderboard.addScore("alice", 100);
        leaderboard.addScore("bob", 200);
        leaderboard.addScore("carol", 150);

        Leaderboard.Entry[5] memory board = leaderboard.getLeaderboard();
        assertEq(board[0].player, "bob");
        assertEq(board[0].score, 200);
        assertEq(board[1].player, "carol");
        assertEq(board[1].score, 150);
        assertEq(board[2].player, "alice");
        assertEq(board[2].score, 100);
    }

    // ---- Access control ----

    function test_RevertWhen_NonOwnerAddsScore() public {
        vm.prank(stranger);

        vm.expectRevert(abi.encodeWithSelector(Ownable.OwnableUnauthorizedAccount.selector, stranger));
        leaderboard.addScore("eve", 999);
    }

    // ---- Edge cases ----

    function test_AddScore_RejectsBelowThreshold() public {
        for (uint256 i = 0; i < 5; i++) {
            leaderboard.addScore("player", 100);
        }
        bool inserted = leaderboard.addScore("loser", 50);
        assertFalse(inserted);
    }

    function test_AddScore_ZeroScoreRejectedOnEmptyBoard() public {
        bool inserted = leaderboard.addScore("nobody", 0);
        assertFalse(inserted);
    }

    function test_AddScore_TieAtCapacityIsRejected() public {
        leaderboard.addScore("p1", 100);
        leaderboard.addScore("p2", 90);
        leaderboard.addScore("p3", 80);
        leaderboard.addScore("p4", 70);
        leaderboard.addScore("p5", 60);
        // board is now full: [100, 90, 80, 70, 60]

        bool inserted = leaderboard.addScore("p6", 60); // ties the lowest entry
        assertFalse(inserted);

        Leaderboard.Entry[5] memory board = leaderboard.getLeaderboard();
        assertEq(board[4].player, "p5"); // incumbent keeps its spot
    }

    function test_AddScore_TieScoreInsertedWithoutDisplacingIncumbent() public {
        leaderboard.addScore("first", 100);
        leaderboard.addScore("second", 100); // ties, but board has room

        Leaderboard.Entry[5] memory board = leaderboard.getLeaderboard();
        assertEq(board[0].player, "first"); // incumbent keeps the higher rank
        assertEq(board[1].player, "second"); // tying entry still gets in
    }

    function test_AddScore_FullBoardEvictsLowest() public {
        leaderboard.addScore("p1", 50);
        leaderboard.addScore("p2", 60);
        leaderboard.addScore("p3", 70);
        leaderboard.addScore("p4", 80);
        leaderboard.addScore("p5", 90);
        // board is now full: [90, 80, 70, 60, 50]

        bool inserted = leaderboard.addScore("p6", 65);
        assertTrue(inserted);

        Leaderboard.Entry[5] memory board = leaderboard.getLeaderboard();
        assertEq(board[0].player, "p5"); // unaffected
        assertEq(board[1].player, "p4"); // unaffected
        assertEq(board[2].player, "p3"); // unaffected
        assertEq(board[3].player, "p6"); // newly inserted
        assertEq(board[3].score, 65);
        assertEq(board[4].player, "p2"); // shifted down from rank 3
        assertEq(board[4].score, 60);
        // p1 (score 50) has been evicted entirely - no longer present anywhere
    }

    event ScoreAdded(string player, uint256 score, uint256 rank);
}
