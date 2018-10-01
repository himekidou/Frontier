using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
//
public class FirebaseMgr : MonoBehaviour
{

    FirebaseAuth auth;
    public static FirebaseUser user;

    FirebaseApp firebaseApp;
    DatabaseReference databaseReference;

    DatabaseReference DBUser;
    DatabaseReference DBInventory;

    public int NickNameNumber = 10498;
    public string NickName = "user-";

    public int SceneCheck;
    public Scene NowScene;
    public delegate void Change();
    public static event Change TimeChanged;


    public string[] GetExpTable;
    public int LevelTable = 1001;

    public string GetGameMoney;
    public string GetRealMoney;
    public string GetLevel;
    public string GetExp;
    public string GetRankPoint;
    public string GetNormalGame1vs1Count;
    public string GetNormalGame1vs1vs1Count;
    public string GetNormalGame1vs1vs1vs1Count;
    public string GetNormalGame2vs2Count;
    public string GetRankGame1vs1Count;
    public string GetRankGame1vs1vs1Count;
    public string GetRankGame1vs1vs1vs1Count;
    public string GetRankGame2vs2Count;
    public string GetNormalGame1vs1Win;
    public string GetNormalGame1vs1vs1Win;
    public string GetNormalGame1vs1vs1vs1Win;
    public string GetNormalGame2vs2Win;
    public string GetRankGame1vs1Win;
    public string GetRankGame1vs1vs1Win;
    public string GetRankGame1vs1vs1vs1Win;
    public string GetRankGame2vs2Win;
    public string GetUserNickName;
    public string GetIntroduceMyself;


    void Awake()
    {
        // 초기화

        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        firebaseApp = FirebaseDatabase.DefaultInstance.App;
        firebaseApp.SetEditorDatabaseUrl("https://sample-212205.firebaseio.com/");
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;


    }
    private void Update()
    {
        //LoadDatabaseScene();
        //SceneManager.sceneLoaded += OnLevelFinishedLoading;
        //SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        //SceneManager.activeSceneChanged += OnLevelFinishedLoading;

    }


    /*
    // 버튼이 눌리면 실행할 함수.
    public void JoinBtnOnClick()
    {
        email = inputTextEmail.text;
        password = inputTextPassword.text;

        Debug.Log("email: " + email + ", password: " + password);

        CreateUser();
    }

    
    void CreateUser()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                loginResult.text = "회원가입 실패";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                loginResult.text = "회원가입 실패";
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            loginResult.text = "회원가입 굿럭";
        });
        
    }
    */
    //버튼 클릭시 구글 로그인
    public void GoogleLoginBtnOnClick()
    {
        GooglePlayServiceInitialize();

        Social.localUser.Authenticate(success =>
        {
            if (success == false) return;

            StartCoroutine(CoLogin());

        });


    }
    //구글 로그인 초기화
    void GooglePlayServiceInitialize()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    // 구글 로그인
    IEnumerator CoLogin()
    {
        while (System.String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;

        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        string accessToken = null;


        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            //AddUid();
            //GetUsers();
            OnClickDataRead();
        });
    }
    //로그아웃
    public void FirebaseLogout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    //현재 유저 아이디 불러오기
    public string GetFirebaseUser()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        string uid = null;
        if (user != null)
        {
            uid = user.UserId;
        }
        return uid;
    }

    //로그인 상태 확인
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }

    }
    //기존 유저인지 신규 유저인지 구분해 데이터를 초기화 하거나 불러옴.
    public void OnClickDataRead()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users") // 읽어올 데이터 이름
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("DataRead Error");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log(snapshot.ChildrenCount);
                    Debug.Log(snapshot.HasChild(GetFirebaseUser()));

                    // DataSnapshot 타입에 저장된 값 불러오기
                    foreach (var item in snapshot.Children)
                    {
                        //snapshot의 키값을 불러와야한다. 그냥 쓰면 키와 밸류가 다나옴
                        if (snapshot.HasChild(GetFirebaseUser()) == false && snapshot.Child(GetFirebaseUser()).ChildrenCount <= 0)
                        {
                            Debug.Log("newbee");
                            DBUser = databaseReference.Child("users").Child(GetFirebaseUser());
                            DBInventory = databaseReference.Child("users").Child(GetFirebaseUser()).Child("Inventory").Child("WeaponID");
                            DBUser.Child("GameMoney").SetValueAsync(0);
                            DBUser.Child("RealMoney").SetValueAsync(0);
                            DBUser.Child("Level").SetValueAsync(1);
                            DBUser.Child("Exp").SetValueAsync(0);
                            DBUser.Child("RankPoint").SetValueAsync(0);
                            DBUser.Child("1vs1NormalGameCount").SetValueAsync(0);
                            DBUser.Child("1vs1NormalGameWin").SetValueAsync(0);
                            DBUser.Child("1vs1vs1NormalGameCount").SetValueAsync(0);
                            DBUser.Child("1vs1vs1NormalGameWin").SetValueAsync(0);
                            DBUser.Child("1vs1vs1vs1NormalGameCount").SetValueAsync(0);
                            DBUser.Child("1vs1vs1vs1NormalGameWin").SetValueAsync(0); ;
                            DBUser.Child("2vs2NormalGameCount").SetValueAsync(0);
                            DBUser.Child("2vs2NormalGameWin").SetValueAsync(0);
                            DBUser.Child("1vs1RankGameCount").SetValueAsync(0);
                            DBUser.Child("1vs1RankGameWin").SetValueAsync(0);
                            DBUser.Child("1vs1vs1RankGameCount").SetValueAsync(0);
                            DBUser.Child("1vs1vs1RankGameWin").SetValueAsync(0);
                            DBUser.Child("1vs1vs1vs1RankGameCount").SetValueAsync(0);
                            DBUser.Child("1vs1vs1vs1RankGameWin").SetValueAsync(0);
                            DBUser.Child("2vs2RankGameCount").SetValueAsync(0);
                            DBUser.Child("2vs2RankGameWin").SetValueAsync(0);
                            DBUser.Child("IntroduceMyself").SetValueAsync("");
                            DBInventory.Child("WID0001").Child("Type").SetValueAsync("Rocket");
                            DBInventory.Child("WID0001").Child("WeaponName").SetValueAsync("Bazooka");
                            DBInventory.Child("WID0001").Child("Force").SetValueAsync(30);
                            DBInventory.Child("WID0001").Child("ActingPower").SetValueAsync(30);
                            DBInventory.Child("WID0001").Child("Explosion").SetValueAsync(1);
                            DBInventory.Child("WID0001").Child("UseTurn").SetValueAsync(1);
                            DBInventory.Child("WID0001").Child("CoolTime").SetValueAsync(0);
                            DBInventory.Child("WID0001").Child("Frequency").SetValueAsync(0);
                            DBInventory.Child("Win").Child("WID0001").SetValueAsync(0);
                            SearchNickName();

                        }
                        else if (snapshot.Child(GetFirebaseUser()).ChildrenCount > 0)
                        {
                            Debug.Log("has data");
                            Debug.Log((snapshot.Child(GetFirebaseUser()).Child("Level").Value));
                            SceneManager.LoadScene("lobby");
                            LoadUserDatabase();
                            break;
                        }
                    }
                }
            });
    }

    //닉네임 테이블을 불러와 중복이 없을 시 집어넣음
    public void SearchNickName()
    {

        Debug.Log("NickName");
        FirebaseDatabase.DefaultInstance
            .GetReference("NickName") // 읽어올 데이터 이름
            .GetValueAsync().ContinueWith(task =>
            {
                Debug.Log("Task");
                if (task.IsFaulted)
                {
                    Debug.LogError("NickName Faulted");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log("Completed");
                    // DataSnapshot 타입에 저장된 값 불러오기


                    for (int i = 0; i <= snapshot.ChildrenCount; i++)
                    {
                        if (snapshot.HasChild(NickName + NickNameNumber))
                        {
                            Debug.Log("if is true");
                            Debug.Log(snapshot.Child(NickName + NickNameNumber).Key.ToString().Trim());
                            Debug.Log((NickName + NickNameNumber).ToString().Trim());
                            NickNameNumber++;

                        }
                        else
                        {   //규칙 주의
                            Debug.Log("else");
                            string DBNickName = NickName + NickNameNumber;
                            Debug.Log(snapshot.HasChild(DBNickName));
                            if (snapshot.HasChild(DBNickName) == true)
                            {
                                Debug.Log("hasChild NickName");
                                SceneManager.LoadScene("lobby");
                                LoadUserDatabase();

                                break;
                            }
                            databaseReference.Child("NickName").Child(DBNickName).SetValueAsync("On");
                            databaseReference.Child("users").Child(GetFirebaseUser()).Child("NickName").SetValueAsync(DBNickName);
                            SceneManager.LoadScene("lobby");
                            LoadUserDatabase();
                            break;

                        }
                    }



                }
            });


    }

    public void LoadUserDatabase()
    {

        //DBUser = databaseReference.Child("users").Child(GetFirebaseUser());

        FirebaseDatabase.DefaultInstance
            .GetReference("users").Child(GetFirebaseUser())
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("NickName Faulted");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    //Debug.Log(snapshot.Child("NickName").Value.ToString());
                    //NickNameText = snapshot.Child("NickName").Value.ToString();

                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().UserNickName = snapshot.Child("NickName").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().GameMoney = snapshot.Child("GameMoney").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RealMoney = snapshot.Child("RealMoney").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().Level = snapshot.Child("Level").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().Exp = snapshot.Child("Exp").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankPoint = snapshot.Child("RankPoint").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().NormalGame1vs1Count = snapshot.Child("1vs1NormalGameCount").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().NormalGame1vs1vs1Count = snapshot.Child("1vs1vs1NormalGameCount").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().NormalGame2vs2Count = snapshot.Child("2vs2NormalGameCount").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankGame1vs1Count = snapshot.Child("1vs1RankGameCount").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankGame1vs1vs1Count = snapshot.Child("1vs1vs1RankGameCount").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankGame1vs1vs1vs1Count = snapshot.Child("1vs1vs1vs1RankGameCount").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankGame2vs2Count = snapshot.Child("2vs2RankGameCount").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().NormalGame1vs1Win = snapshot.Child("1vs1NormalGameWin").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().NormalGame1vs1vs1Win = snapshot.Child("1vs1vs1NormalGameWin").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().NormalGame1vs1vs1vs1Win = snapshot.Child("1vs1vs1vs1NormalGameWin").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().NormalGame2vs2Win = snapshot.Child("2vs2NormalGameWin").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankGame1vs1Win = snapshot.Child("1vs1RankGameWin").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankGame1vs1vs1Win = snapshot.Child("1vs1vs1RankGameWin").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankGame1vs1vs1vs1Win = snapshot.Child("1vs1vs1vs1RankGameWin").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().RankGame2vs2Win = snapshot.Child("2vs2RankGameWin").Value.ToString();
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().IntroduceMyself = snapshot.Child("IntroduceMyself").Value.ToString();

                    LoadExpDatabase();
                    //LoadInventoryDatabase();
                }
            });


    }

    public void LoadExpDatabase()
    {

        //DBUser = databaseReference.Child("users").Child(GetFirebaseUser());

        FirebaseDatabase.DefaultInstance
            .GetReference("Exp").Child("ExpTable")
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("ExpTable Faulted");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log("in task");
                    GetExpTable = new string[30];
                    foreach (var item in snapshot.Children)
                    {
                        Debug.Log("foreach");
                        GetExpTable = (string[])(item.Child("Level-" + LevelTable).Value);
                        Debug.Log(GetExpTable);
                        LevelTable++;

                    }
                    GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().ExpTabel = GetExpTable;



                }
            });


    }
    /*
    public void LoadInventoryDatabase()
    {
        FirebaseDatabase.DefaultInstance
           .GetReference("users").Child(GetFirebaseUser()).Child("Inventory")
           .GetValueAsync().ContinueWith(task =>
           {
               if (task.IsFaulted)
               {
                   Debug.LogError("InventoryData Faulted");
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot snapshot = task.Result;
                   Debug.Log("in task");
                   GetExpTable = new string[30];
                   foreach (var item in snapshot.Children)
                   {
                       int i = 0;
                       GetExpTable[i] = item.Child("Level-" + LevelTable).Value.ToString();
                       LevelTable++;
                       i++;

                   }
                   GameObject.Find("UserDBMgr").GetComponent<UserDBMgr>().ExpTabel = GetExpTable;



               }
           });
    }
    public void UpdateIntroduce()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(GetFirebaseUser());

        reference.ValueChanged += HandleValueChanged;
    }


    public void OnClickEventListner()
    {

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(GetFirebaseUser());

        reference.ValueChanged += HandleValueChanged;
    }

    //users 데이터 전체의 변화에 수신
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        SnapshotAllDataRead(args.Snapshot);
    }


    // 데이터를 읽어온다.
    public void SnapshotDataRead(DataSnapshot Snapshot)
    {
        DataSnapshot snapshot = Snapshot;

        foreach (var users in snapshot.Children)
        {
            Debug.Log(users.Value);
        }
    }

    public void SnapshotAllDataRead(DataSnapshot Snapshot)
    {
        foreach (var users in Snapshot.Children)
        {
            Debug.Log(users.Child(GetFirebaseUser()).Value);
        }
    }
    */

}