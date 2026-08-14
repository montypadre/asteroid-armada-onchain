// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import {Script, console} from "forge-std/Script.sol";
import {Leaderboard} from "../src/Leaderboard.sol";
import {GameLog} from "../src/GameLog.sol";

contract Deploy is Script {
    function run() external {
        uint256 deployerPrivateKey = vm.envUint("PRIVATE_KEY");

        vm.startBroadcast(deployerPrivateKey);

        Leaderboard leaderboard = new Leaderboard();
        GameLog gameLog = new GameLog();

        vm.stopBroadcast();

        console.log("Leaderboard deployed to:", address(leaderboard));
        console.log("GameLog deployed to:", address(gameLog));
    }
}
