# Asteroid Armada — On-Chain Contracts

A 2026 modernization of two Solidity contracts originally written in 2021 for **Asteroid Armada**, a Unity game built as part of the HyperJump/Thugs DAO project on Binance Smart Chain. This repo rebuilds `Leaderboard.sol` and `GameLog.sol` to current Solidity standards, keeping the original design intent while fixing real bugs, adding a full test suite, and deploying to Base Sepolia — a deliberate "then and now" exercise.

The original, unmodified 2021 contracts are preserved at **[github.com/montypadre/hypr](https://github.com/montypadre/hypr)** for direct comparison.

## Live, verified contracts (Base Sepolia)

| Contract | Address | Verified source |
|---|---|---|
| `Leaderboard` | `0xFb960560a320A59Fa19626013CaC9E1950056f42` | [Basescan](https://sepolia.basescan.org/address/0xfb960560a320a59fa19626013cac9e1950056f42#code) |
| `GameLog` | `0x142B6835f1A1C5a5c33e23C23BffD98129e55Cb7` | [Basescan](https://sepolia.basescan.org/address/0x142b6835f1a1c5a5c33e23c23bffd98129e55cb7#code) |

## Then vs. now

| | 2021 original | 2026 rewrite |
|---|---|---|
| Solidity version | `pragma solidity >=0.7.4;` (open-ended) | `pragma solidity ^0.8.24;` (pinned, matches `foundry.toml`) |
| Access control | Hand-rolled `owner` variable + `onlyOwner` modifier | OpenZeppelin `Ownable` / `AccessControl` |
| Leaderboard storage | `mapping(uint256 => User)` used as a pseudo-array | Fixed `Entry[5]` array — real, compiler-enforced bounds |
| Leaderboard insert bug | Forward swap-chain loop touches an out-of-bounds 6th slot (silent on a mapping; would revert on a real array) | Backward shift bounded to `rank..BOARD_SIZE-1` — structurally cannot overrun |
| GameLog storage | 3 of 4 struct fields commented out; only `gameSeed` ever persisted | Full struct (`player`, `score`, `gameSeed`) written on every call |
| GameLog replay check | `for (i = 0; i < 100; i++)` re-checking the identical condition — advisory only, never enforced | `mapping(uint256 => bool)` — O(1), enforced atomically inside `logGame` |
| GameLog permissions | Single `gameServeAddress` slot, one address at a time | `AccessControl` `GAME_SERVER_ROLE` — any number of holders, independently managed |
| Error handling | `require(cond, "string")` | Custom errors (`OwnableUnauthorizedAccount`, `SeedAlreadyUsed`, etc.) |
| Events | None | `ScoreAdded`, `GameLogged` |
| Tests | None | 17 Foundry tests — happy paths, access control, replay rejection, leaderboard edge cases |
| Target chain | Binance Smart Chain | Base Sepolia (Base-mainnet-ready) |
| Deployment | Manual | `forge script` with committed on-chain broadcast records |
| Verification | None evident | Verified on Basescan via the Etherscan V2 unified API |
| Client integration | C# / Nethereum (Unity) | TypeScript / viem |

## Project structure
├── src/
│ ├── Leaderboard.sol # Top-5 owner-submitted score board
│ └── GameLog.sol # Game-session log with anti-replay protection
├── test/
│ ├── Leaderboard.t.sol # 9 tests
│ └── GameLog.t.sol # 8 tests
├── script/
│ └── Deploy.s.sol # Foundry deploy script
├── client/ # viem/TypeScript integration client
│ └── index.ts
└── broadcast/ # Foundry's on-chain deployment records

## Getting started
Requires [Foundry](https://getfoundry.sh).
```bash
forge install
forge build
forge test
Test suite
Ran 9 tests for test/Leaderboard.t.sol:LeaderboardTest
[PASS] test_AddScore_EmitsScoreAddedEvent() (gas: 81947)
[PASS] test_AddScore_FullBoardEvictsLowest() (gas: 305893)
[PASS] test_AddScore_InsertsIntoEmptyBoard() (gas: 91328)
[PASS] test_AddScore_MaintainsDescendingOrder() (gas: 193481)
[PASS] test_AddScore_RejectsBelowThreshold() (gas: 268910)
[PASS] test_AddScore_TieAtCapacityIsRejected() (gas: 284749)
[PASS] test_AddScore_TieScoreInsertedWithoutDisplacingIncumbent() (gas: 141564)
[PASS] test_AddScore_ZeroScoreRejectedOnEmptyBoard() (gas: 10329)
[PASS] test_RevertWhen_NonOwnerAddsScore() (gas: 13931)
Suite result: ok. 9 passed; 0 failed; 0 skipped; finished in 6.62ms (2.22ms CPU time)

Ran 8 tests for test/GameLog.t.sol:GameLogTest
[PASS] test_Admin_CanGrantAndRevokeGameServerRole() (gas: 35722)
[PASS] test_LogGame_EmitsGameLoggedEvent() (gas: 108099)
[PASS] test_LogGame_MarksSeedAsUsed() (gas: 107936)
[PASS] test_LogGame_PersistsAllFields() (gas: 107914)
[PASS] test_RevertWhen_NonAdminGrantsRole() (gas: 18056)
[PASS] test_RevertWhen_SeedReplayed() (gas: 109007)
[PASS] test_RevertWhen_SeedReplayedByDifferentServer() (gas: 141261)
[PASS] test_RevertWhen_StrangerLogsGame() (gas: 17087)
Suite result: ok. 8 passed; 0 failed; 0 skipped; finished in 6.62ms (1.85ms CPU time)

Ran 2 test suites in 15.73ms (13.24ms CPU time): 17 tests passed, 0 failed, 0 skipped (17 total tests)

Deployment
Deployed via script/Deploy.s.sol and verified independently per-contract:

forge script script/Deploy.s.sol:Deploy --rpc-url base_sepolia --broadcast
forge verify-contract <address> src/Leaderboard.sol:Leaderboard --chain 84532 --watch
forge verify-contract <address> src/GameLog.sol:GameLog --chain 84532 --watch
TypeScript client
A minimal viem script demonstrating external client integration — connects to the deployed Leaderboard, submits a score, and reads the board back. The direct 2026 counterpart to the original Unity game's C#/Nethereum integration.

cd client
npm install
npm start
License
MIT

Once you've filled in the `GameLog` test output, create the file, then:
```bash
git add README.md
git commit -m "Add README with before/after modernization story"
git push