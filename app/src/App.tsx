import { useAccount, useConnect, useDisconnect } from 'wagmi'
import UnityGame from './UnityGame'

function App() {
  const { address, isConnected } = useAccount()
  const { connect, connectors, isPending } = useConnect()
  const { disconnect } = useDisconnect()

  return (
    <div style={{ padding: '2rem', fontFamily: 'sans-serif' }}>
      <h1>Asteroid Armada</h1>

      {isConnected ? (
        <div>
          <p>Connected: {address}</p>
          <button onClick={() => disconnect()}>Disconnect</button>
        </div>
      ) : (
        <div>
          {connectors.map((connector) => (
            <button
              key={connector.uid}
              onClick={() => connect({ connector })}
              disabled={isPending}
            >
              Connect {connector.name}
            </button>
          ))}
        </div>
      )}

      <UnityGame />
    </div>
  )
}

export default App