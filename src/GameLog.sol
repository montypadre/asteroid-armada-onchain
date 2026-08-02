// SPDX-License-Identifier: SEE LICENSE IN LICENSE
pragma solidity ^0.8.24;

import {AccessControl} from "@openzeppelin/contracts/access/AccessControl.sol";

/// @title GameLog
/// @notice On-chain event log for Asteroid Armada game sessions, with anti-replay protection on game seeds.
/// @dev 2026 rewrite of a 2021 alpha-stage original whose storage writes were stubbed out
///      and whose seed-replay check was non-functional. Both are fixed here.
contract GameLog is AccessControl {
    /// @notice Role granted to backend game-server address authorized to log game results.
    bytes32 public constant GAME_SERVER_ROLE = keccak256("GAME_SERVER_ROLE");

    /// @notice Identifier for the game this deployment logs (fixed per-deployment, not per-call).
    string public constant GAME_ID = "asteroids";

    /// @notice A single logged game session.
    struct GameInfo {
        address player;
        uint256 score;
        uint256 gameSeed;
    }

    /// @notice Game logs keyed by the (unique) seed used for that session.
    mapping(uint256 => GameInfo) private gameLog;

    /// @notice 0(1) anti-replay lookup: has this seed already been logged?
    mapping(uint256 => bool) private usedSeeds;

    /// @notice Emitted when a game session is successfully logged.
    /// @param player The player's address.
    /// @param score The score achieved.
    /// @param gameSeed The unique seed for this session.
    event GameLogged(address indexed player, uint256 score, uint256 gameSeed);

    /// @notice Reverts if `gameSeed` has already been logged.
    error SeedAlreadyUsed(uint256 gameSeed);

    /// @notice Deployer becomes the initial admin, matching the original contract's "deployer is owner" behavior.
    constructor() {
        _grantRole(DEFAULT_ADMIN_ROLE, msg.sender);
    }

    /// @notice Record the result of a completed game session.
    /// @dev Only callable by addresses holding GAME_SERVER_ROLE. Reverts if the seed was
    ///      already used, so the same session can never be logged twice.
    /// @param player Address the session is attributed to.
    /// @param score Score achieved in the session.
    /// @param gameSeed Unique seed identifying this session.
    function logGame(address player, uint256 score, uint256 gameSeed) external onlyRole(GAME_SERVER_ROLE) {
        if (usedSeeds[gameSeed]) {
            revert SeedAlreadyUsed(gameSeed);
        }

        usedSeeds[gameSeed] = true;
        gameLog[gameSeed] = GameInfo({player: player, score: score, gameSeed: gameSeed});

        emit GameLogged(player, score, gameSeed);
    }

    /// @notice Check whether a game seed has already been logged.
    /// @param gameSeed The seed to check.
    /// @return used True if the seed has already been used.
    function isGameSeedUsed(uint256 gameSeed) external view returns (bool used) {
        return usedSeeds[gameSeed];
    }

    /// @notice Retrieve a previously logged game session.
    /// @param gameSeed The seed identifying the session.
    /// @return info The logged game session data.
    function getGameInfo(uint256 gameSeed) external view returns (GameInfo memory info) {
        return gameLog[gameSeed];
    }
}
