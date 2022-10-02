using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

// [ExecuteInEditMode]
public class ArtNet:MonoBehaviour
{

    [SerializeField]
    private string _destinationIP = "127.0.0.1";

    [SerializeField]
    private byte _universe = 0x0;

    [SerializeField]
    private float _outputHz = 44;

    [SerializeField]
    [Range(0,255)]
    private byte[] _data = new byte[512];


    private UdpClient _socket;
    private IPEndPoint _target;

    private byte[] _artNetPacket = new byte[530];

    private float _lastTxTime = 0;
    private float _intervalTime;

    public void Start()
    {
        _target = new IPEndPoint(IPAddress.Parse(_destinationIP), 6454);

        _socket = new UdpClient();
        _socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _socket.Connect(_target);

        string str = "Art-Net";
        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        encoding.GetBytes(str, 0, str.Length, _artNetPacket, 0);

        _artNetPacket[7] = 0x0;

        //opcode low byte first
        _artNetPacket[8] = 0x00;
        _artNetPacket[9] = 0x50;

        //proto ver high byte first
        _artNetPacket[10] = 0x0;
        _artNetPacket[11] = 0x14;

        //TODO: Full Addressing

        //sequence 
        _artNetPacket[12] = 0x0;

        //physical port
        _artNetPacket[13] = 0x0;

        //universe low byte first
        _artNetPacket[14] = _universe;
        _artNetPacket[15] = 0x0;

        //length high byte first
        _artNetPacket[16] = ((512 >> 8) & 0xFF);
        _artNetPacket[17] = (512 & 0xFF);
    }

    private void tx()
    {
        Buffer.BlockCopy(_data, 0, _artNetPacket, 18, 512);

        try
        {
            _socket.Send(_artNetPacket, _artNetPacket.Length);
        }
        catch (Exception e)
        {
            Debug.Log (this +" "+ e);
        }
    }

    public void Update()
    {
        _intervalTime = 1f / _outputHz;

        if (Time.time - _lastTxTime >= _intervalTime)
        {
            _lastTxTime = Time.time;
            tx();
        }
    }

    public bool setChannel(int channel, int value)
    {
        // Debug.Log("set Channel: " + channel + " set Value: " + value);
        if (channel < 1) return false;
        if (channel > 511) return false;
        if (value < 0) return false;
        if (value > 255) return false;

        _data[channel-1] = Convert.ToByte(value);
        return true;
    }

    public void setDestiniationIP(string val)
    {
        _destinationIP = val;
    }
}