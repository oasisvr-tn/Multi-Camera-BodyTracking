using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
public class Socket : MonoBehaviour
{
    Thread mThread;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector3[] receivedPos=new Vector3[14];
    Vector3[] previousPos=new Vector3[14];
    bool running;
    public float speed;
    private void Update()
    {
        float step=speed * Time.deltaTime;
        if (true)
        {
            previousPos[0]=transform.Find("nose").position;
            previousPos[1]=transform.Find("r_shoulder").position;
            previousPos[2]=transform.Find("l_shoulder").position;
            previousPos[3]=transform.Find("r_elbow").position;
            previousPos[4]=transform.Find("l_elbow").position;
            previousPos[5]=transform.Find("r_hand").position;
            previousPos[6]=transform.Find("l_hand").position;
            previousPos[7]=transform.Find("r_hip").position;
            previousPos[8]=transform.Find("l_hip").position;
            previousPos[9]=transform.Find("r_knee").position;
            previousPos[10]=transform.Find("l_knee").position;
            previousPos[11]=transform.Find("r_foot").position;
            previousPos[12]=transform.Find("l_foot").position;
            previousPos[13]=transform.Find("neck").position;
        }
        transform.Find("nose").position=Vector3.MoveTowards(previousPos[0],receivedPos[0],step);
        transform.Find("r_shoulder").position=Vector3.MoveTowards(previousPos[1],receivedPos[1],step);
        transform.Find("l_shoulder").position=Vector3.MoveTowards(previousPos[2],receivedPos[2],step);
        transform.Find("r_elbow").position=Vector3.MoveTowards(previousPos[3],receivedPos[3],step);
        transform.Find("l_elbow").position=Vector3.MoveTowards(previousPos[4],receivedPos[4],step);
        transform.Find("r_hand").position=Vector3.MoveTowards(previousPos[5],receivedPos[5],step);
        transform.Find("l_hand").position=Vector3.MoveTowards(previousPos[6],receivedPos[6],step);
        transform.Find("r_hip").position=Vector3.MoveTowards(previousPos[7],receivedPos[7],step);
        transform.Find("l_hip").position=Vector3.MoveTowards(previousPos[8],receivedPos[8],step);
        transform.Find("r_knee").position=Vector3.MoveTowards(previousPos[9],receivedPos[9],step);
        transform.Find("l_knee").position=Vector3.MoveTowards(previousPos[10],receivedPos[10],step);
        transform.Find("r_foot").position=Vector3.MoveTowards(previousPos[11],receivedPos[11],step);
        transform.Find("l_foot").position=Vector3.MoveTowards(previousPos[12],receivedPos[12],step);
        transform.Find("neck").position=Vector3.MoveTowards(previousPos[13],receivedPos[13],step);

    }
    private void Start()
    {
        ThreadStart ts=new ThreadStart(GetInfo);
        mThread=new Thread(ts);
        mThread.Start();
    }
    void GetInfo()
    {
        localAdd=IPAddress.Parse("127.0.0.1");
        listener=new TcpListener(IPAddress.Any,25003);
        listener.Start();
        client=listener.AcceptTcpClient();
        running=true;
        while (running){SendAndReceiveData();}
        listener.Stop();
    }
    void SendAndReceiveData()
    {
        NetworkStream nwStream=client.GetStream();
        byte[] buffer=new byte[client.ReceiveBufferSize];
        int bytesRead=nwStream.Read(buffer,0,client.ReceiveBufferSize);
        string dataReceived=Encoding.UTF8.GetString(buffer,0,bytesRead);
        if (dataReceived != null)
        {
            receivedPos=StringToVector3(dataReceived);
        }
    } 
    public Vector3[] StringToVector3(string sVector)
    {
        if (sVector.StartsWith("[") && sVector.EndsWith("]"))
        {
            sVector=sVector.Substring(1,sVector.Length - 2);
        }
        Vector3[] positionArray=new Vector3[14];
        string[] sArray=sVector.Split(';');
        for (int i=0;i<sArray.Length;i++)
        {
            if (sVector.StartsWith("[") && sVector.EndsWith("]"))
            {
                sArray[i]=sArray[i].Substring(1,sArray[i].Length-2);
            }
            string[] sArray2=sArray[i].Split(',');
            positionArray[i]=new Vector3(float.Parse(sArray2[0]),float.Parse(sArray2[2]),float.Parse(sArray2[1]));
        }
        return positionArray;
    }
}
