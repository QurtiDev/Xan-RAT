


namespace InvokedCommon.Enums
{
	public enum ConnectionState : byte
	{
		Closed = 1,
		Listening = 2,
		SYN_Sent = 3,
		Syn_Recieved = 4,
		Established = 5,
		Finish_Wait_1 = 6,
		Finish_Wait_2 = 7,
		Closed_Wait = 8,
		Closing = 9,
		Last_ACK = 10, // 0x0A
		Time_Wait = 11, // 0x0B
		Delete_TCB = 12, // 0x0C
	}
}
