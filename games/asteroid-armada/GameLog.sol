pragma solidity >=0.7.4;

contract GameLog {
    
    // person who deploys contract is the owner
    address owner;
    
    address gameServeAddress;
    
    // unique identifier
    string gameId = "asteroids";
    
    // create an array of game logs
    mapping (uint256 => GameInfo) gameLog; 
    
    struct GameInfo {
        string gameId;
        address userAddress;
        uint score;
        uint256 gameSeed;
    }
    
    constructor() public {
        owner = msg.sender;
    }
    
    // allows owner only
    modifier onlyOwner() {
        require(owner == msg.sender, "Sender not authorized");
        _;
    }
    
    // require msg.sender is gameServeAddress address
    modifier gameServe() {
        require(msg.sender == gameServeAddress, "You are not authorized to do this");
        _;
    }
    
   function updateGameInfo (string memory _gameId, address _userAddress, uint _score, uint256 _gameSeed) gameServe() public returns (bool success) {
        //gameLog[_gameSeed].gameId =  _gameId;
        //gameLog[_gameSeed].userAddress =  _userAddress;
        //gameLog[_gameSeed].score =  _score;
        gameLog[_gameSeed].gameSeed = _gameSeed;

        return true;
   }
   
   // FOR TESTING PURPOSES
   //function getInfo (uint256 _gameSeed) public view returns (string memory _gameId, address _userAddress, uint _score) {
      //return (
          //gameLog[_gameSeed].gameId,
          //gameLog[_gameSeed].userAddress,
          //gameLog[_gameSeed].score); 
   //}
   
   function isGameSeedAlreadyUsed(uint256 _gameSeed) gameServe() public view returns (bool) {
       for (uint i = 0; i < 100; i++) {
            if (gameLog[_gameSeed].gameSeed == _gameSeed) {
                return true;
            }
       }
   } 
   
   function updateGameServeAddress(address _gameServeAddress) public onlyOwner {
       gameServeAddress = _gameServeAddress;
   }
}