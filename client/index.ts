import { config } from "dotenv";
config({ path: "../.env" });

import { createPublicClient, createWalletClient, http } from "viem";
import { privateKeyToAccount } from "viem/accounts";
import { baseSepolia } from "viem/chains";

const LEADERBOARD_ADDRESS = "0xFb960560a320A59Fa19626013CaC9E1950056f42" as const;

const leaderboardAbi = [
    {
        type: "function",
        name: "addScore",
        stateMutability: "nonpayable",
        inputs: [
            { name: "player", type: "string" },
            { name: "score", type: "uint256" },
        ],
        outputs: [{ name: "inserted", type: "bool" }],
    },
    {
        type: "function",
        name: "getLeaderboard",
        stateMutability: "view",
        inputs: [],
        outputs: [
            {
                name: "",
                type: "tuple[5]",
                components: [
                    { name: "player", type: "string" },
                    { name: "score", type: "uint256" },
                ],
            },
        ],
    },
] as const;

async function main() {
    const rpcUrl = process.env.BASE_SEPOLIA_RPC_URL;
    const privateKey = process.env.PRIVATE_KEY;

    if (!rpcUrl || !privateKey) {
        throw new Error("Missing BASE_SEPOLIA_RPC_URL or PRIVATE_KEY in .env");
    }

    const account = privateKeyToAccount(privateKey as `0x${string}`);

    const publicClient = createPublicClient({
        chain: baseSepolia,
        transport: http(rpcUrl),
    });

    const walletClient = createWalletClient({
        account,
        chain: baseSepolia,
        transport: http(rpcUrl),
    });

    const player = "TS-Client-Player";
    const score = BigInt(Math.floor(Math.random() * 10_000));

    console.log(`Submitting score ${score} for "${player}" as ${account.address}...`);

    const hash = await walletClient.writeContract({
        address: LEADERBOARD_ADDRESS,
        abi: leaderboardAbi,
        functionName: "addScore",
        args: [player, score],
    });

    console.log(`Transaction submitted: ${hash}`);
    console.log("Waiting for confirmation...");

    const receipt = await publicClient.waitForTransactionReceipt({ hash, confirmations: 2 });
    console.log(`Confirmed in block ${receipt.blockNumber}\n`);

    const board = await publicClient.readContract({
        address: LEADERBOARD_ADDRESS,
        abi: leaderboardAbi,
        functionName: "getLeaderboard",
    });

    console.log("Current Leaderboard:");
    board.forEach((entry, i) => {
        if (entry.score > 0n) {
            console.log(`   ${i + 1}. ${entry.player} - ${entry.score.toString()}`);
        }
    });
}

main().catch((err) => {
    console.error(err);
    process.exit(1);
});