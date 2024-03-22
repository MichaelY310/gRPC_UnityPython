using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Net.Client;
using GrpcGreeter;
using System;
using UnityEditor.PackageManager;
using System.Net;
using Grpc.Net.Client.Web;
using Grpc.Core;
using System.Net.Http;
using Cysharp.Net.Http;
using static GrpcGreeter.Greeter;

public class Client : MonoBehaviour
{
    [SerializeField]
    Transform cube;

    private YetAnotherHttpHandler handler;
    private GrpcChannel channel;
    private Greeter.GreeterClient client;
    private float newPosition_x;
    private float newPosition_z;

/*    void Start()
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        using var channeltest = GrpcChannel.ForAddress("http://localhost:5071", new GrpcChannelOptions() { HttpHandler = handler });
        var clienttest = new Greeter.GreeterClient(channeltest);

        InputPosition request = new InputPosition { X = 0, Z = 0 };
        var reply = clienttest.RequestInstruction(request);
    }*/

    // Start is called before the first frame update
    void Start()
    {
        handler = new YetAnotherHttpHandler() { Http2Only = true };
        channel = GrpcChannel.ForAddress("http://localhost:5071", new GrpcChannelOptions() { HttpHandler = handler });
        client = new Greeter.GreeterClient(channel);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            GetResponseAsync();
        }
        cube.position = new Vector3(newPosition_x, 0, newPosition_z);
    }

    async void GetResponseAsync()
    {
        InputPosition request = new InputPosition { X = cube.position.x, Z = cube.position.z };
        OutputAI response = await client.RequestInstructionAsync(request);
        newPosition_x = response.Fx;
        newPosition_z = response.Fz;
        print("Greeting: " + response.Fx + " " + response.Fz);

    }

    private void GetResponse()
    {
        InputPosition request = new InputPosition { X = cube.position.x, Z = cube.position.z };
        OutputAI response = client.RequestInstruction(request);
        newPosition_x = response.Fx;
        newPosition_z = response.Fz;
        print("Greeting: " + response.Fx + " " + response.Fz);

    }

    private void OnDestroy()
    {
        channel.Dispose();
    }
}
