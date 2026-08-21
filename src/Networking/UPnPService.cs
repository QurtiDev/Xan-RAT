

using Open.Nat;
using System;
using System.Collections.Generic;
using System.Threading;


namespace InvokedServer.Networking
{
	public class UPnPService
	{
		private readonly Dictionary<int, Mapping> _mappings = new Dictionary<int, Mapping>();
		private NatDevice _device;
		private NatDiscoverer _discoverer;

		public UPnPService() => this._discoverer = new NatDiscoverer();

		public async void CreatePortMapAsync(int port)
		{
			try
			{
				this._device = await this._discoverer.DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource(10000));
				Mapping mapping = new Mapping(Protocol.Tcp, port, port);
				await this._device.CreatePortMapAsync(mapping);
				if (this._mappings.ContainsKey(mapping.PrivatePort))
					this._mappings[mapping.PrivatePort] = mapping;
				else
					this._mappings.Add(mapping.PrivatePort, mapping);
				mapping = (Mapping)null;
			}
			catch (Exception ex) when (ex is MappingException || ex is NatDeviceNotFoundException)
			{
			}
		}

		public async void DeletePortMapAsync(int port)
		{
			Mapping mapping;
			if (!this._mappings.TryGetValue(port, out mapping))
			{
				mapping = (Mapping)null;
			}
			else
			{
				try
				{
					await this._device.DeletePortMapAsync(mapping);
					this._mappings.Remove(mapping.PrivatePort);
					mapping = (Mapping)null;
				}
				catch (Exception ex) when (ex is MappingException || ex is NatDeviceNotFoundException)
				{
					mapping = (Mapping)null;
				}
			}
		}
	}
}