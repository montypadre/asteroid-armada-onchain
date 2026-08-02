// SPDX-License-Identifier: SEE LICENSE IN LICENSE
pragma solidity ^0.8.24;

import {Ownable} from "@openzeppelin/contracts/access/Ownable.sol";

/// @title Leaderboard
/// @notice On-chain top-5 high score board for Asteroid Armada.
/// @dev 2026 rewrite of a 2021 original. Score submission stays owner-gated -
///      the intent is a trusted backend validates gameplay off-chain and
///      submits the resulting score, not that players call this directly.
contract Leaderboard is Ownable {
    /// @notice Number of ranked slots tracked on the board.
    uint256 public constant BOARD_SIZE = 5;

    /// @notice A single leaderboard entry.
    struct Entry {
        string player;
        uint256 score;
    }

    /// @notice The top BOARD_SIZE entries, sorted descending by score (index 0 = highest).
    Entry[BOARD_SIZE] private leaderboard;

    /// @notice Emitted when a score is accepted onto the board.
    /// @param player Display name associated with the score.
    /// @param score The submitted score.
    /// @param rank Zero-based rank the score was inserted at (0 = 1st place).
    event ScoreAdded(string player, uint256 score, uint256 rank);

    /// @notice Deployer becomes the intial owner, matching the original contract's behavior.
    constructor() Ownable(msg.sender) {}

    /// @notice Submit a candidate score for leaderboard inclusion.
    /// @dev A score must be strictly greater than the current lowest entry to be inserted;
    ///      ties favor whichever entry is already on the board.
    /// @param player Display name to record alongside the score.
    /// @param score The score to submit.
    /// @return inserted True if the score was high enough to be added to the board.
    function addScore(string calldata player, uint256 score) external onlyOwner returns (bool inserted) {
        if (score <= leaderboard[BOARD_SIZE - 1].score) {
            return false;
        }

        uint256 rank = 0;
        while (rank < BOARD_SIZE && score <= leaderboard[rank].score) {
            rank++;
        }

        for (uint256 j = BOARD_SIZE - 1; j > rank; j--) {
            leaderboard[j] = leaderboard[j - 1];
        }

        leaderboard[rank] = Entry({player: player, score: score});

        emit ScoreAdded(player, score, rank);
        return true;
    }

    /// @notice Read the full leaderboard in one call.
    /// @return The current top BOARD_SIZE entries, descending by score.
    function getLeaderboard() external view returns (Entry[BOARD_SIZE] memory) {
        return leaderboard;
    }
}
