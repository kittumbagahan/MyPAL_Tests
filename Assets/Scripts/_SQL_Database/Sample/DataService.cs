﻿using SQLite4Unity3d;
using UnityEngine;
using System;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataService
{

    public SQLiteConnection _connection { private set; get; }

    public DataService()
    {
        string DatabaseName; /* = PlayerPrefs.GetString("activeDatabase") == "" ? "tempDatabase.db" : PlayerPrefs.GetString("activeDatabase");*/
        if (PlayerPrefs.GetString("activeDatabase") == "")
        {
            PlayerPrefs.SetString("activeDatabase", "tempDatabase.db");
            DatabaseName = "tempDatabase.db";
        }
        else
        {
            DatabaseName = PlayerPrefs.GetString("activeDatabase");
        }
        string dbPath = Application.persistentDataPath + "/" + DatabaseName;

//#if UNITY_EDITOR
//        //var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
//#else
//        // check if file exists in Application.persistentDataPath
//        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

//        if (!File.Exists(filepath))
//        {
//            Debug.Log("Database not in Persistent path");
//            // if it doesn't ->
//            // open StreamingAssets directory and load the db ->

//#if UNITY_ANDROID
//            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
//            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
//            // then save to Application.persistentDataPath
//            File.WriteAllBytes(filepath, loadDb.bytes);
//#elif UNITY_IOS
//                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//                // then save to Application.persistentDataPath
//                File.Copy(loadDb, filepath);
//#elif UNITY_WP8
//                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//                // then save to Application.persistentDataPath
//                File.Copy(loadDb, filepath);

//#elif UNITY_WINRT
//		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//		// then save to Application.persistentDataPath
//		File.Copy(loadDb, filepath);
		
//#elif UNITY_STANDALONE_OSX
//		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//		// then save to Application.persistentDataPath
//		File.Copy(loadDb, filepath);
//#else
//	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
//	// then save to Application.persistentDataPath
//	File.Copy(loadDb, filepath);

//#endif

//            Debug.Log("Database written");
//        }

//        var dbPath = filepath;
//#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);

    }

    public DataService(string DatabaseName)
    {

#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);

    }


    public void CreateDB()
    {
        Debug.Log(Application.persistentDataPath);
        if ("".Equals(PlayerPrefs.GetString("deviceId_created")))
        {
            //do we need to save?
            PlayerPrefs.SetString("deviceId_created", SystemInfo.deviceUniqueIdentifier);
        }

        if (0.Equals(PlayerPrefs.GetInt("device_table_created")))
        {
            _connection.CreateTable<DeviceModel>();
            //add UID through networking
            PlayerPrefs.SetInt("device_table_created", 1);
        }

        if (0.Equals(PlayerPrefs.GetInt("section_table_created")))
        {
            _connection.CreateTable<SectionModel>();
            SectionModel sectionModel = new SectionModel
            {
                Id = 0,
                DeviceId = "9999",
                Description = "Diamond"
            };
            SectionModel sectionModel2 = new SectionModel
            {
                Id = 0,
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Description = "Gold"
            };
            SectionModel sectionModel3 = new SectionModel
            {
                Id = 1,
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Description = "Diamond"
            };
            _connection.Insert(sectionModel);
            _connection.Insert(sectionModel2);
            _connection.Insert(sectionModel3);
            PlayerPrefs.SetInt("section_table_created", 1);
        }

        if (0.Equals(PlayerPrefs.GetInt("student_table_created")))
        {
            _connection.CreateTable<StudentModel>();
            PlayerPrefs.SetInt("student_table_created", 1);
        }

        if (0.Equals(PlayerPrefs.GetInt("book_table_created")))
        {
            _connection.CreateTable<BookModel>();

            _connection.InsertAll(new[] {
                   new BookModel
                   {

                      Description = StoryBook.ABC_CIRCUS.ToString()
                   },
                    new BookModel
                   {

                      Description = StoryBook.AFTER_THE_RAIN.ToString()
                   },
                       new BookModel
                   {

                      Description = StoryBook.CHAT_WITH_MY_CAT.ToString()
                   },
                  new BookModel
                   {

                      Description = StoryBook.COLORS_ALL_MIXED_UP.ToString()
                   },
                     new BookModel
                   {

                      Description = StoryBook.FAVORITE_BOX.ToString()
                   },
                     new BookModel
                   {

                      Description = StoryBook.JOEY_GO_TO_SCHOOL.ToString()
                   },
                  new BookModel
                   {

                      Description = StoryBook.SOUNDS_FANTASTIC.ToString()
                   },
                     new BookModel
                   {

                      Description = StoryBook.TINA_AND_JUN.ToString()
                   },
                        new BookModel
                   {

                      Description = StoryBook.WHAT_DID_YOU_SEE.ToString()
                   },
                           new BookModel
                   {

                      Description = StoryBook.YUMMY_SHAPES.ToString()
                   }
             });


            PlayerPrefs.SetInt("book_table_created", 1);
        }

        if (0.Equals(PlayerPrefs.GetInt("activity_table_created")))
        {
            _connection.CreateTable<ActivityModel>();

            //NOTE: THE BOOK ID BASED ON THE BOOK TABLE CREATION

            //ABC-CIRCUS
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act2",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act2",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act2",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act2",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act2",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act4",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act6",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act5",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act5",
                    Module = "PUZZLE",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act1",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act1",
                    Module = "OBSERVATION",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act1",
                    Module = "OBSERVATION",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act1",
                    Module = "OBSERVATION",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 1,
                    Description = "ABCCircus_Act1",
                    Module = "OBSERVATION",
                    Set = 12
                },
            });

            //AFTER THE RAIN
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act1",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act1",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act1",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act1",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act1",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act2",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act3",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act3",
                    Module = "PUZZLE",
                    Set = 4
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act4",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act6",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act6",
                    Module = "OBSERVATION",
                    Set = 4
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act6",
                    Module = "OBSERVATION",
                    Set = 10
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act6",
                    Module = "OBSERVATION",
                    Set = 18
                },
                new ActivityModel
                {
                    BookId = 2,
                    Description = "afterTheRain_Act6",
                    Module = "OBSERVATION",
                    Set = 28
                },
            });

            //CHAT WITH MY CAT
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_2",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_2",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_2",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_2",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_2",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_3",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_4",
                    Module = "PUZZLE",
                    Set = 1
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_5",
                    Module = "PUZZLE",
                    Set = 2
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_6",
                    Module = "PUZZLE",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_1",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_1",
                    Module = "OBSERVATION",
                    Set = 1
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_1",
                    Module = "OBSERVATION",
                    Set = 2
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_1",
                    Module = "OBSERVATION",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 3,
                    Description = "chatWithCat_Act_1",
                    Module = "OBSERVATION",
                    Set = 4
                },
            });

            //COLORS ALL MIXED UP
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_7",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_7",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_7",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_7",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_7",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_2",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_3",
                    Module = "PUZZLE",
                    Set = 1
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_4",
                    Module = "PUZZLE",
                    Set = 2
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_5",
                    Module = "PUZZLE",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_6",
                    Module = "PUZZLE",
                    Set = 4
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_1",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_1",
                    Module = "OBSERVATION",
                    Set = 1
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_1",
                    Module = "OBSERVATION",
                    Set = 2
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_1",
                    Module = "OBSERVATION",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 4,
                    Description = "colorsAllMixedUp_Act_1",
                    Module = "OBSERVATION",
                    Set = 4
                },
            });

            //FAVORITE BOX
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act1_word",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act1_word",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act1_word",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act1_word",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act1_word",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act2_coloring",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act4",
                    Module = "PUZZLE",
                    Set = 1
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act5",
                    Module = "PUZZLE",
                    Set = 2
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favbox6_NEW",
                    Module = "PUZZLE",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act3_spotDiff",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act3_spotDiff",
                    Module = "OBSERVATION",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act3_spotDiff",
                    Module = "OBSERVATION",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act3_spotDiff",
                    Module = "OBSERVATION",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 5,
                    Description = "favBox_Act3_spotDiff",
                    Module = "OBSERVATION",
                    Set = 12
                },
            });

            //JOEY GOES TO SCHOOL
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act1",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act1",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act1",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act1",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act1",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act2",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act4",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act5",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act6",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act3",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act3",
                    Module = "OBSERVATION",
                    Set = 4
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act3",
                    Module = "OBSERVATION",
                    Set = 10
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act3",
                    Module = "OBSERVATION",
                    Set = 18
                },
                new ActivityModel
                {
                    BookId = 6,
                    Description = "JoeyGoesToSchool_Act3",
                    Module = "OBSERVATION",
                    Set = 28
                },
            });

            //SOUNDS FANTASTIC
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act4",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act4",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act4",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act4",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act4",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "soundsFantastic_Act1",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 7,
                    Description = "soundsFantastic_Act2",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act5",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act5",
                    Module = "PUZZLE",
                    Set = 4
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act3",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act6",
                    Module = "OBSERVATION",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act7",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act8",
                    Module = "OBSERVATION",
                    Set = -1
                },
                new ActivityModel
                {
                    BookId = 7,
                    Description = "SoundsFantastic_Act8",
                    Module = "OBSERVATION",
                    Set = 2
                },
            });

            //TINA AND JUN
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act1",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act2",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act2",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act2",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act2",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act4",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act5",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act6",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act7",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act3",
                    Module = "OBSERVATION",
                    Set = -1
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act3",
                    Module = "OBSERVATION",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act3",
                    Module = "OBSERVATION",
                    Set = 7
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act3",
                    Module = "OBSERVATION",
                    Set = 11
                },
                new ActivityModel
                {
                    BookId = 8,
                    Description = "TinaAndJun_Act3",
                    Module = "OBSERVATION",
                    Set = 15
                },
            });

            //WHAT DID YOU SEE
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct7",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct7",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct7",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct7",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct7",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "whatDidYaSee_act1",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct4",
                    Module = "PUZZLE",
                    Set = 1
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct6",
                    Module = "PUZZLE",
                    Set = 2
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct3",
                    Module = "PUZZLE",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "whatDidYaSee_act2",
                    Module = "OBSERVATION",
                    Set = -1
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "whatDidYaSee_act2",
                    Module = "OBSERVATION",
                    Set = 2
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "whatDidYaSee_act2",
                    Module = "OBSERVATION",
                    Set = 5
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct5",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 9,
                    Description = "WhatDidYouSeeAct8",
                    Module = "OBSERVATION",
                    Set = 0
                },
            });

            //YUMMY SHAPES
            _connection.InsertAll(new[] {
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_1",
                    Module = "WORD",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_1",
                    Module = "WORD",
                    Set = 3
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_1",
                    Module = "WORD",
                    Set = 6
                },
                new ActivityModel
                {
                   BookId = 10,
                    Description = "yummyShapes_Act_1",
                    Module = "WORD",
                    Set = 9
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_1",
                    Module = "WORD",
                    Set = 12
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_4",
                    Module = "PUZZLE",
                    Set = 0
                },
                 new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_5",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_6",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_7",
                    Module = "PUZZLE",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_2",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_2",
                    Module = "OBSERVATION",
                    Set = 3
                },
                new ActivityModel
                {
                   BookId = 10,
                    Description = "yummyShapes_Act_3",
                    Module = "OBSERVATION",
                    Set = 0
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_3",
                    Module = "OBSERVATION",
                    Set = 4
                },
                new ActivityModel
                {
                    BookId = 10,
                    Description = "yummyShapes_Act_3",
                    Module = "OBSERVATION",
                    Set = 8
                },
            });

            PlayerPrefs.SetInt("activity_table_created", 1);
        }

        //--------------------------------------------

        if (0.Equals(PlayerPrefs.GetInt("studentActivityModel_table_created")))
        {
            _connection.CreateTable<StudentActivityModel>();
            PlayerPrefs.SetInt("studentActivityModel_table_created", 1);
        }

        if (0.Equals(PlayerPrefs.GetInt("studentBookModel_table_created")))
        {
            _connection.CreateTable<StudentBookModel>();
            PlayerPrefs.SetInt("studentBookModel_table_created", 1);
        }



    }

    public IEnumerable<BookModel> GetBooks()
    {
        return _connection.Table<BookModel>();
    }

    public IEnumerable<ActivityModel> GetActivities()
    {
        return _connection.Table<ActivityModel>();
    }

    public IEnumerable<SectionModel> GetSections()
    {
        return _connection.Table<SectionModel>();
    }

    public IEnumerable<StudentModel> GetStudents()
    {
        return _connection.Table<StudentModel>();
    }

    public IEnumerable<StudentActivityModel> GetStudentActivities()
    {
        return _connection.Table<StudentActivityModel>();
    }

    public IEnumerable<StudentBookModel> GetStudentBooks()
    {
        return _connection.Table<StudentBookModel>();
    }
    public IEnumerable<Person> GetPersonsNamedRoberto()
    {
        return _connection.Table<Person>().Where(x => x.Name == "Roberto");
    }

    public Person GetJohnny()
    {
        return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
    }

    public Person CreatePerson()
    {
        var p = new Person
        {
            Name = "Johnny",
            Surname = "Mnemonic",
            Age = 21
        };
        _connection.Insert(p);
        return p;
    }
}
