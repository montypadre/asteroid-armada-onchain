// SPDX-License-Identifier: SEE LICENSE IN LICENSE
pragma solidity ^0.8.24;

import {Test} from "forge-std/Test.sol";
import {GameLog} from "../src/GameLog.sol";
import {IAccessControl} from "@openzeppelin/contracts/access/IAccessControl.sol";

contract GameLogTest is Test {
    GameLog gameLog;
    address gameServer = makeAddr("gameServer");
    address stranger = makeAddr("stranger");
    address player = makeAddr("player");

    function setUp() public {
        gameLog = new GameLog();
        gameLog.grantRole(gameLog.GAME_SERVER_ROLE(), gameServer);
    }

    // ---- Happy path ----

    function test_LogGame_PersistsAllFields() public {
        vm.prank(gameServer);
        gameLog.logGame(player, 500, 12345);

        GameLog.GameInfo memory info = gameLog.getGameInfo(12345);
        assertEq(info.player, player);
        assertEq(info.score, 500);
        assertEq(info.gameSeed, 12345);
    }

    function test_LogGame_EmitsGameLoggedEvent() public {
        vm.expectEmit(true, true, true, true);
        emit GameLogged(player, 500, 12345);

        vm.prank(gameServer);
        gameLog.logGame(player, 500, 12345);
    }

    function test_LogGame_MarksSeedAsUsed() public {
        assertFalse(gameLog.isGameSeedUsed(999));

        vm.prank(gameServer);
        gameLog.logGame(player, 10, 999);

        assertTrue(gameLog.isGameSeedUsed(999));
    }

    // ---- Access control ----

    function test_RevertWhen_StrangerLogsGame() public {
        bytes32 role = gameLog.GAME_SERVER_ROLE();

        vm.prank(stranger);
        vm.expectRevert(
            abi.encodeWithSelector(IAccessControl.AccessControlUnauthorizedAccount.selector, stranger, role)
        );
        gameLog.logGame(player, 10, 1);
    }

    function test_RevertWhen_NonAdminGrantsRole() public {
        bytes32 gameServerRole = gameLog.GAME_SERVER_ROLE();
        bytes32 adminRole = gameLog.DEFAULT_ADMIN_ROLE();

        vm.prank(stranger);
        vm.expectRevert(
            abi.encodeWithSelector(IAccessControl.AccessControlUnauthorizedAccount.selector, stranger, adminRole)
        );
        gameLog.grantRole(gameServerRole, stranger);
    }

    function test_Admin_CanGrantAndRevokeGameServerRole() public {
        address newServer = makeAddr("newServer");
        assertFalse(gameLog.hasRole(gameLog.GAME_SERVER_ROLE(), newServer));

        gameLog.grantRole(gameLog.GAME_SERVER_ROLE(), newServer);
        assertTrue(gameLog.hasRole(gameLog.GAME_SERVER_ROLE(), newServer));

        gameLog.revokeRole(gameLog.GAME_SERVER_ROLE(), newServer);
        assertFalse(gameLog.hasRole(gameLog.GAME_SERVER_ROLE(), newServer));
    }

    // ---- Replay rejection ----

    function test_RevertWhen_SeedReplayed() public {
        vm.prank(gameServer);
        gameLog.logGame(player, 100, 42);

        vm.prank(gameServer);
        vm.expectRevert(abi.encodeWithSelector(GameLog.SeedAlreadyUsed.selector, uint256(42)));
        gameLog.logGame(player, 200, 42);
    }

    function test_RevertWhen_SeedReplayedByDifferentServer() public {
        address secondServer = makeAddr("secondServer");
        gameLog.grantRole(gameLog.GAME_SERVER_ROLE(), secondServer);

        vm.prank(gameServer);
        gameLog.logGame(player, 100, 7);

        vm.prank(secondServer);
        vm.expectRevert(abi.encodeWithSelector(GameLog.SeedAlreadyUsed.selector, uint256(7)));
        gameLog.logGame(player, 999, 7);
    }

    event GameLogged(address indexed plyaer, uint256 score, uint256 gameSeed);
}
