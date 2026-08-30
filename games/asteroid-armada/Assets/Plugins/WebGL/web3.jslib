mergeInto(LibraryManager.library, {

  RequestLeaderboardJS: function () {
    console.log("[Web3Bridge] RequestLeaderboardJS() called from Unity");

    // TODO (Phase 3): replace this stub with a real viem readContract called
    // to Leaderboard.getLeaderboard(), made from the React host page.
    setTimeout(function () {
      var fakeResponse = {
        entries: [
          { player: "Stub-Player-1", score: 9999 },
          { player: "Stub-Player-2", score: 8100 },
          { player: "Stub-Player-3", score: 4028 },
          { player: "", score: 0 },
          { player: "", score: 0 }
        ]
      };

      if (window.unityInstance) {
        window.unityInstance.SendMessage("Web3Bridge", "ReceiveLeaderboard", JSON.stringify(fakeResponse));
      } else {
        console.error("[Web3Bridge] window.unityInstance is not available - cannot send data back to Unity.");
      }
    }, 500);
  },

  SubmitScoreJS: function (playerNamePtr, score) {
    var playerName = UTF8ToString(playerNamePtr);
    console.log("[Web3Bridge] SubmitScoreJS() called from Unity with player=" + playerName + "score=" + score);

    // TODO (Phase 3): replace this stub with a real viem WriteContract call
    // to Leaderboard.addScore(), signed by the wallet connected in the React host.
    setTimeout(function () {
      var fakeResult = {
        success: true,
        message: "Stub submission accepted (no real transaction sent yet)",
        txHash: "0xSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBSTUBST"
      };

      if (window.unityInstance) {
        window.unityInstance.SendMessage("Web3Bridge", "ReceiveScoreSubmissionResult", JSON.stringify(fakeResult));
      } else {
        console.error("[Web3Bridge] window.unityInstance is not available - cannot send data back to Unity.");
      }
    }, 500);
  },

});