﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UserBookSave : MonoBehaviour {

    //REFERENCE FROM CarouItem.cs StoryBookSaveManager.instance.selectedBookName = sceneToLoad;

    //DataService ds = new DataService();
    DataService ds;

    StudentBookModel model;

    // network data
    NetworkData networkData;

    void Start()
    {
        // kit
        ds = new DataService();

        string bookname = StoryBookSaveManager.ins.selectedBook.ToString();

        BookModel book = ds._connection.Table<BookModel>().Where(x => x.Description == bookname).FirstOrDefault();

        model = ds._connection.Table<StudentBookModel>().Where(x=> x.SectionId == StoryBookSaveManager.ins.activeSection_id &&
        x.StudentId == StoryBookSaveManager.ins.activeUser_id && 
        x.BookId == book.Id).FirstOrDefault();

        if (model == null)
        {
            model = new StudentBookModel
            {
                SectionId = StoryBookSaveManager.ins.activeSection_id,
                StudentId = StoryBookSaveManager.ins.activeUser_id,
                BookId = book.Id,
                ReadCount = 0,
                ReadToMeCount = 0,
                AutoReadCount = 0
            };

            networkData = new NetworkData ();
            networkData.studentBook_SectionId = model.SectionId;
            networkData.studentBook_StudentId = model.StudentId;
            networkData.studentBook_bookId = model.BookId;
            networkData.studentBook_readCount = model.ReadCount;
            networkData.studentBook_readToMeCount = model.ReadToMeCount;
            networkData.studentBook_autoReadCount = model.AutoReadCount;

            ds._connection.Insert(model);
        }
    }

    public void UpdateReadUsage()
    { 
       //reading key
        //string key = "read" + "section_id" + StoryBookSaveManager.ins.activeSection_id + "student_id" + StoryBookSaveManager.ins.activeUser_id
        //  + StoryBookSaveManager.ins.selectedBook;
        ////print("Read Usage " + key);
        //PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
        int count = model.ReadCount + 1;
        ds._connection.Execute("Update StudentBookModel set ReadCount='" + count + "' where Id='" + model.Id + "'");

        if(networkData != null)
        {
            // network data
            networkData.studentBook_Id = model.Id;
            networkData.studentBook_readCount = count;

            if (MainNetwork.Instance.clientSendFile.isActiveAndEnabled)
                MainNetwork.Instance.clientSendFile.SendData(networkData, ClientSendFile.MessageGroup.Book_UpdateReadCount);
        }
      
    }

    public void UpdateReadItToMeUsage()
    {
        //2018 08 30//string key = "readItToMe" + StoryBookSaveManager.ins.oldUsername + StoryBookSaveManager.ins.selectedBook;
        //string key = "readItToMe" + "section_id" + StoryBookSaveManager.ins.activeSection_id + "student_id" + StoryBookSaveManager.ins.activeUser_id
        //    + StoryBookSaveManager.ins.selectedBook;
        //PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
        //print("Read Usage " + key);
        int count = model.ReadToMeCount + 1;
        ds._connection.Execute("Update StudentBookModel set ReadToMeCount='" + count + "' where Id='" + model.Id + "'");

        // network data
        networkData.studentBook_Id = model.Id;
        networkData.studentBook_readCount = count;

        if (MainNetwork.Instance.clientSendFile.isActiveAndEnabled)
            MainNetwork.Instance.clientSendFile.SendData(networkData, ClientSendFile.MessageGroup.Book_UpdateReadToMeCount);
    }

    public void UpdateAutoReadUsage()
    {
        //2018 08 30//string key = "auto" + StoryBookSaveManager.ins.oldUsername + StoryBookSaveManager.ins.selectedBook;
        //string key = "auto" + "section_id" + StoryBookSaveManager.ins.activeSection_id + "student_id" + StoryBookSaveManager.ins.activeUser_id
        //    + StoryBookSaveManager.ins.selectedBook;
        //PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
        //print("Read Usage " + key);
        int count = model.AutoReadCount + 1;
        ds._connection.Execute("Update StudentBookModel set AutoReadCount='" + count + "' where Id='" + model.Id + "'");

        // network data
        networkData.studentBook_Id = model.Id;
        networkData.studentBook_readCount = count;

        if (MainNetwork.Instance.clientSendFile.isActiveAndEnabled)
            MainNetwork.Instance.clientSendFile.SendData(networkData, ClientSendFile.MessageGroup.Book_UpdateAutoReadCount);
    }


 

}
