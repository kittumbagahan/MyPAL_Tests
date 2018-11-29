﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class EnableGames : MonoBehaviour {

    public static EnableGames ins;

    void Start()
    {
        ins = this;
        if (IsAvailable() == 0)
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
        }


    }


    int IsAvailable()
    {
        DataService ds = new DataService();
        string bookname = StoryBookSaveManager.ins.selectedBook.ToString();
        BookModel book = ds._connection.Table<BookModel>().Where(x => x.Description == bookname).FirstOrDefault();
        var model = ds._connection.Table<StudentBookModel>().Where(x => x.SectionId == StoryBookSaveManager.ins.activeSection_id &&
        x.StudentId == StoryBookSaveManager.ins.activeUser_id && x.BookId == book.Id).FirstOrDefault();

        if (model == null) return 0;

        return model.ReadCount + model.ReadToMeCount + model.AutoReadCount;
    }
}
