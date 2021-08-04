namespace Server.General {
	internal static class ConnectionStatuses: object {
		internal enum ConnectionStatus: byte {
			Waiting,
			Connecting,
			Connected,
			JoiningLobby,
			JoinedLobby,
			JoiningRoom,
			JoinedRoom,
			Amt
		}
	}
}