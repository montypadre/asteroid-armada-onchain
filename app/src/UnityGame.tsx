import { useCallback, useEffect } from 'react'
import { Unity, useUnityContext } from 'react-unity-webgl'
import { useCall } from 'wagmi'

function UnityGame() {
    const { unityProvider, addEventListener, removeEventListener, sendMessage } = useUnityContext({
        loaderUrl: '/games/asteroid-armada/Build/Web.loader.js',
        dataUrl: '/games/asteroid-armada/Build/Web.data',
        frameworkUrl: '/games/asteroid-armada/Build/Web.framework.js',
        codeUrl: '/games/asteroid-armada/Build/Web.wasm',
    })

    const handleRequestLeaderboard = useCallback(() => {
        // TODO (next step): replace with a real viem readContract call to Leaderboard.getLeaderboard()
        const fakeResponse = {
            entries: [
                { player: 'Stub-Player-1', score: 9999 },
                { player: 'Stub-Player-2', score: 8100 },
                { player: 'Stub-Player-3', score: 4028 },
                { player: '', score: 0},
                { player: '', score: 0},
            ],
        }
        sendMessage('Web3Bridge', 'ReceiveLeaderboard', JSON.stringify(fakeResponse))
    }, [sendMessage])

    const handleSubmitScore = useCallback((playerName: unknown, score: unknown) => {
        // TODO (next step): replace with a real viem writeContract call to Leaderboard.addScore()
        console.log('[UnityGame] SubmitScore event received:', playerName, score)
        const fakeResult = {
            success: true,
            message: 'Stub submission accepted (no real transaction sent yet)',
            txHash: '0xSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBST',
        }
        sendMessage('Web3Bridge', 'ReceiveScoreSubmissionResult', JSON.stringify(fakeResult))
    }, [sendMessage])

    useEffect(() => {
        addEventListener('RequestLeaderboard', handleRequestLeaderboard)
        addEventListener('SubmitScore', handleSubmitScore)
        return () => {
            removeEventListener('RequestLeaderboard', handleRequestLeaderboard)
            removeEventListener('SubmitScore', handleSubmitScore)
        }
    }, [addEventListener, removeEventListener, handleRequestLeaderboard, handleSubmitScore])

    return (
        <Unity
            unityProvider={unityProvider}
            style={{ width: '960px', height: '600px' }}
        />
    )
}

export default UnityGame