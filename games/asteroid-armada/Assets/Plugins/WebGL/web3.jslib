mergeInto(LibraryManager.library, {

  RequestLeaderboardJS: function () {
    console.log("[Web3Bridge] RequestLeaderboardJS() called from Unity");
    window.dispatchReactUnityEvent("RequestLeaderboard");
  },

  SubmitScoreJS: function (playerNamePtr, score) {
    var playerName = UTF8ToString(playerNamePtr);
    console.log("[Web3Bridge] SubmitScoreJS() called from Unity with player=" + playerName + "score=" + score);
    window.dispatchReactUnityEvent("SubmitScore", playerName, score);
  },

});