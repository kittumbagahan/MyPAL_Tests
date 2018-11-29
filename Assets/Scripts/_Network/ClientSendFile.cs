﻿using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System.IO;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ClientSendFile : MonoBehaviour
{
    // kit
    [SerializeField]
    UnityEngine.UI.Text txtTest;

    public enum MessageGroup
    {
        Insert = 2,
        Update = 3,
        Book_UpdateReadCount = 4,
        Book_UpdateReadToMeCount = 5,
        Book_UpdateAutoReadCount = 6
    }

    Queue<NetworkData> networkQueue;

    DataService dataService;

    private void Start()
    {        
        NetworkManager.Instance.Networker.binaryMessageReceived += ReceiveFile;
        networkQueue = new Queue<NetworkData>();

        // create database connection
        dataService = new DataService();
    }
    
    private void ReceiveFile(NetworkingPlayer player, Binary frame, NetWorker sender)
    {
        if (frame.GroupId != MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Book_UpdateAutoReadCount &&
            frame.GroupId != MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Book_UpdateReadCount &&
            frame.GroupId != MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Book_UpdateReadToMeCount &&
            frame.GroupId != MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Insert &&
            frame.GroupId != MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Update)
            return;        

        Debug.Log("Reading file!");

        // kit
        Debug.Log(string.Format("Insert group id {0}\nUpdate group id {1}", 
            (int)MessageGroup.Insert,
            (int)MessageGroup.Update));
        Debug.Log(string.Format("Message group {0}", frame.GroupId));

        NetworkData networkData = ConvertToObject(frame.StreamData.CompressBytes());
        // add to queue for execution
        networkQueue.Enqueue(networkData);

        while (networkQueue.Count > 0)
        {
            // kit
            Debug.Log("Queue count " + networkQueue.Count);

            if (frame.GroupId == MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Update ||
               frame.GroupId == MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Insert)
            {
                // activity model

                string module = networkQueue.Peek ().activity_module;
                string description = networkQueue.Peek ().activity_description;
                int set = networkQueue.Peek ().activity_set;
                string book_description = networkQueue.Peek ().book_description;

                var activity = dataService._connection.Table<ActivityModel> ().Where (x => x.Module == module &&
                                                                                      x.Description == description &&
                                                                                      x.Set == set).FirstOrDefault ();

                if (activity == null)
                {
                    var _activity = new ActivityModel
                    {
                        BookId = dataService._connection.Table<BookModel> ().Where (x => x.Description == book_description).FirstOrDefault ().Id,
                        Description = networkQueue.Peek ().activity_description,
                        Module = networkQueue.Peek ().activity_module,
                        Set = networkQueue.Peek ().activity_set
                    };
                    dataService._connection.Insert (_activity);
                }
            }

            // if message is insert
            if (frame.GroupId == MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Insert)
            {

                // kit
                Debug.Log("Insert");
                // handle insert here, check first item in queue
                StudentActivityModel studentActivityModel = new StudentActivityModel
                {
                    Id = networkQueue.Peek().studentActivity_ID,
                    SectionId = networkQueue.Peek().studentActivity_sectionId,
                    StudentId = networkQueue.Peek().studentActivity_studentId,
                    BookId = networkQueue.Peek().studentActivity_bookId,
                    ActivityId = networkQueue.Peek().studentActivity_activityId,
                    Grade = networkQueue.Peek().studentActivity_grade,
                    PlayCount = networkQueue.Peek().studentActivity_playCount
                };

                // kit
                Debug.Log(string.Format("ID {0}\nSection ID {1}\nStudent ID {2}\nBook ID {3}\nActivity ID {4}\nGrade {5}\nPlay Count {6}",
                    studentActivityModel.Id,
                    studentActivityModel.SectionId,
                    studentActivityModel.StudentId,
                    studentActivityModel.BookId,
                    studentActivityModel.ActivityId,
                    studentActivityModel.Grade,
                    studentActivityModel.PlayCount));

                dataService._connection.Insert(studentActivityModel);
                networkQueue.Dequeue();

            }
            else
            {
                // handle update here
                string command = "";
                if (frame.GroupId == MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Update)
                {
                    command = string.Format("Update StudentActivityModel set Grade='{0}'," +
                    "PlayCount='{1}' where Id='{2}'", networkData.studentActivity_grade, networkData.studentActivity_playCount, networkData.studentActivity_ID);
                }
                else if (frame.GroupId == MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Book_UpdateReadCount)
                {
                    Debug.Log ("Update read count");
                    if (CreateStudentBookModel (networkQueue.Peek ()) == false)
                    {
                        command = string.Format ("Update StudentBookModel set ReadCount='{0}' where id='{1}'",
                            networkData.studentBook_readCount,
                            networkData.studentBook_Id);

                        dataService._connection.Execute (command);
                    }
                }
                else if (frame.GroupId == MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Book_UpdateReadToMeCount)
                {
                    Debug.Log ("Update read to me count");
                    if (CreateStudentBookModel (networkQueue.Peek ()) == false)
                    {
                        command = string.Format ("Update StudentBookModel set ReadToMeCount='{0}' where id='{1}'",
                        networkData.studentBook_readToMeCount,
                        networkData.studentBook_Id);

                        dataService._connection.Execute (command);
                    }
                }
                else if (frame.GroupId == MessageGroupIds.START_OF_GENERIC_IDS + (int)MessageGroup.Book_UpdateAutoReadCount)
                {
                    Debug.Log ("Update auto read count");
                    if (CreateStudentBookModel (networkQueue.Peek ()) == false)
                    {
                        command = string.Format ("Update StudentBookModel set AutoReadCount='{0}' where id='{1}'",
                        networkData.studentBook_autoReadCount,
                        networkData.studentBook_Id);

                        dataService._connection.Execute (command);
                    }
                }                

                // kit
                Debug.Log("Update");
                Debug.Log(command);

                //dataService._connection.Execute(command);
                networkQueue.Dequeue();
            }
        }

        // kit
        Debug.Log("Queue empty");

		// kit, test data display text
		//MainThreadManager.Run( () => GameObject.FindGameObjectWithTag("data").GetComponent<UnityEngine.UI.Text>().text = string.Format("Name: {0}\nAge: {1}\nSection: {2}\n\n", networkData.name, networkData.age, networkData.section));

        // Write the rest of the payload as the contents of the file and
        // use the file name that was extracted as the file's name    

        //MainThreadManager.Run(() => File.WriteAllBytes(string.Format("{0}/{1}", Application.persistentDataPath, fileName), frame.StreamData.CompressBytes()));        
    }

    public void SendData(NetworkData pNetworkData, MessageGroup messageGroup)
    {       
        // Throw an error if this is not the server
        var networker = NetworkManager.Instance.Networker;

        // event when file is sent        

        if (networker.IsServer)
        {
            Debug.LogError("Only the client can send files in this example!");
            return;
        }      

		byte[] allData = { };


		// convert pData as byte[]
		BinaryFormatter binFormatter = new BinaryFormatter();
		MemoryStream memStream = new MemoryStream ();
		binFormatter.Serialize (memStream, pNetworkData);

		allData = memStream.ToArray ();

		Debug.Log ("allData " + allData.Length);		

//        // Prepare a byte array for sending
//        BMSByte allData = new BMSByte();        
//
//        // Add the file name to the start of the payload        
//        ObjectMapper.Instance.MapBytes(allData);        

        // Send the file to all connected clients
        Binary frame = new Binary(
            networker.Time.Timestep,                    // The current timestep for this frame
            false,                                      // We are server, no mask needed
            allData,                                    // The file that is being sent
            Receivers.Others,                           // Send to all clients
            MessageGroupIds.START_OF_GENERIC_IDS + (int)messageGroup,   // Some random fake number
            networker is TCPServer);

//        if (networker is UDPServer)
//            ((UDPServer)networker).Send(frame, true);
//        else
//            ((TCPServer)networker).SendAll(frame);

		if (networker is UDPClient)
			((UDPClient)networker).Send (frame, true);
		else
			((TCPClient)networker).Send (frame);
		
						
        //StringBuilder("sending file");
    }

	NetworkData ConvertToObject(byte[] byteData)
	{
		BinaryFormatter bin = new BinaryFormatter ();
		MemoryStream ms = new MemoryStream ();
		ms.Write (byteData, 0, byteData.Length);
		ms.Seek (0, SeekOrigin.Begin);

		return (NetworkData)bin.Deserialize (ms);
	}

    bool CreateStudentBookModel (NetworkData pNetworkData)
    {
        // check student book model
        StudentBookModel studentModel = dataService._connection.Table<StudentBookModel> ().Where
        (
           x => x.SectionId == pNetworkData.studentBook_SectionId &&
           x.StudentId == pNetworkData.studentBook_StudentId &&
           x.BookId == pNetworkData.studentBook_bookId
        ).FirstOrDefault ();

        if (studentModel == null)
        {
            Debug.Log ("Create student book model");
            StudentBookModel studentBookModel = new StudentBookModel
            {
                SectionId = pNetworkData.studentBook_SectionId,
                StudentId = pNetworkData.studentBook_StudentId,
                BookId = pNetworkData.studentBook_bookId,
                ReadCount = pNetworkData.studentBook_readCount,
                ReadToMeCount = pNetworkData.studentBook_readToMeCount,
                AutoReadCount = pNetworkData.studentBook_autoReadCount
            };
            dataService._connection.Insert (studentBookModel);

            return true;
        }
        else
        {
            Debug.Log ("Create student book model update");
            return true;
        }
    }

    // kit, test
    void DebugText(string pText)
    {
        txtTest.text += pText;
    }
}
