mergeInto(LibraryManager.library, {
  WalletAddress: function () {
    var returnStr
    try {
      // get address from metamask
      returnStr = window.ethereum.selectedAddress
    } catch (e) {
      returnStr = ""
    }
    var returnStr = window.ethereum.selectedAddress;
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
});