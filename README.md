# gRPC_UnityPython
A very simple example of building connection between a Unity client and python server.

## How to run
Run "GrpcUnityPy/GrpcPython/greeter_server.py" and then click the play button in Unity. Everytime you press the "Space" button, the cube should move to a random place. 

## Note
* Checkout these two tutorials to learn about environments and gRPC: https://grpc.io/docs/languages/python/basics/    https://learn.microsoft.com/en-us/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-8.0&tabs=visual-studio
* It seems like Unity doesn't support Http/2. So if you got something like "bad response" or "error getting response" before reading this, consider using https://github.com/Cysharp/yetanotherhttphandler.

## 1. Create the project
* Make a folder called "GrpcUnityPy".
* Inside the project folder add "GrpcPython" (server) and "unity_grpc_gen" (for proto generation).
* Create a Unity 3D Core project called "UnityClient". I used Unity 2022.3.19f1.

The project structure should look like this:
```
GrpcUnityPy
  ├── GrpcPython
  ├── unity_grpc_gen
  ├── UnityClient
```

## 2. Create the proto file
In my example I will use the client to send the x and z location of a cube and receive fx and fz from the server.
* Make a file called "helloworld.proto" under "unity_grpc_gen" folder.
* Also copy and paste it under "GrpcPython" for convenience.
```
syntax = "proto3";

option csharp_namespace = "GrpcGreeter";

package greet;

service Greeter {
  rpc RequestInstruction (InputPosition) returns (OutputAI);
}

message InputPosition {
  float x = 1;
  float z = 2;
}

message OutputAI {
  float fx = 1;
  float fz = 2;
}
```

## 3. Python Server
First, run the following commands:
(If you run into problems related to numpy, this issue should help: https://github.com/Unity-Technologies/ml-agents/issues/6047)
```
pip install grpcio-tools
cd GrpcUnityPy/GrpcPython
python -m grpc_tools.protoc  -I ./  --python_out=.  --grpc_python_out=.  ./*.proto
```
Second, create a file called "greeter_server.py" under GrpcPython.
```
from concurrent import futures
import logging

import grpc
import helloworld_pb2
import helloworld_pb2_grpc
import random


class Greeter(helloworld_pb2_grpc.GreeterServicer):
    def RequestInstruction(self, request, context):
        print("received instruction: " + str(request.x) + ", " + str(request.z))
        return helloworld_pb2.OutputAI(fx=random.random(), fz=random.random())

def serve():
    port = "5071"
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    helloworld_pb2_grpc.add_GreeterServicer_to_server(Greeter(), server)
    server.add_insecure_port("[::]:" + port)
    server.start()
    print("Server started, listening on " + port)
    server.wait_for_termination()


if __name__ == "__main__":
    logging.basicConfig()
    serve()

```
You can run both greeter_server.py and greeter_client.py in the repo to test if it works. 

## 4. Unity Client
First, download NuGetForUnity by following the instruction: https://github.com/GlitchEnzo/NuGetForUnity.
Second, download these things by using NuGetForUnity:
![image](https://github.com/MichaelY310/gRPC_UnityPython/assets/95990939/b3fde25d-3c8b-46f4-8acd-a352dce0d0c3)


Third, we need YetAnotherHttpHandler. Download it by following the instruction:  https://github.com/Cysharp/YetAnotherHttpHandler.
Finally, download Grpc.Tools2.62.0 https://www.nuget.org/packages/Grpc.Tools/ (You can also directly grab it from repo under "unity_grpc_gen"). After getting the .nupkg file by clicking Download package, rename it to .zip and put the extracted things under "unity_grpc_gen".

That's all we need. Now we can start coding.
First, run the following commands:
```
cd ../unity_grpc_gen
mkdir client
protoc -I ./ --csharp_out=./client --grpc_out=./client --plugin=protoc-gen-grpc=”grpc_csharp_plugin.exe” helloworld.proto
```
Second, in the Unity project, create a folder called "Scripts" under "Assets". Copy paste the file under "client" to the Script folder in Unity project.
Third, create a .cs file called Client.cs under the Script folder:
```
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
        InputPosition request = new InputPosition { X = 0, Z = 0 };
        OutputAI response = await client.RequestInstructionAsync(request);
        newPosition_x = response.Fx;
        newPosition_z = response.Fz;
        print("Greeting: " + response.Fx + " " + response.Fz);

    }

    private void GetResponse()
    {
        InputPosition request = new InputPosition { X = 0, Z = 0 };
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
```
If your Cysharp namespace looks red in the editor, checkout this issue: https://github.com/dotnet/vscode-csharp/issues/4196.
Also see if https://github.com/Cysharp/YetAnotherHttpHandler/issues/61 helps if you encounter the same problem.

Finally, create a empty object called "Agent" and drag "Client.cs" onto it. Drag the default cube in the Unity scene to here:
![image](https://github.com/MichaelY310/gRPC_UnityPython/assets/95990939/ae62480c-29a2-4709-910f-df39ddc1a030)


## Test
Run "greeter_server.py" and then click the play button in Unity. Everytime you press the "Space" button, the cube should move to a random place. 

## Problems
If your test failed, I would recommand you to try connecting python server and .Net client using the two tutorials I mentioned in the beginning. Make sure both of them use Http which makes things easier.
Also try connecting Unity client to .Net server. This reveals more hint to the problem than using the python server (For example, Http/2 is not supported).
