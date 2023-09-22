using Android.App;
using Android.OS;
using Android.Widget;

using AndroidX.AppCompat.App;

using FCMClient;

using SaikoroTravelApp.Backends;

using SaikoroTravelCommon.Models;

using System;
using System.Linq;

using static Android.Content.ClipData;

using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace SaikoroTravelApp.Activities
{
    [Activity(Label = "RegistorUserActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class RegisterUserActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_user);

			Users[] users = Enum.GetValues(typeof(Users)).Cast<Users>().ToArray();
            var arr = users.Select(x => ModelHelper.GetName(x)).ToArray();
            Spinner select = FindViewById<Spinner>(Resource.Id.UserSelect);
            select.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, arr);

            EditText input = FindViewById<EditText>(Resource.Id.UserConfirmCodeInput);
            input.Text = "デバッグモードのため、どんな値でも通過します";

            Button button = FindViewById<Button>(Resource.Id.RegisterUserButton);
            button.Click += async (sender, e) =>
            {
                button.Enabled = false;

                var index = select.SelectedItemPosition;
                Users item = users[index];
                var code = input.Text;

                /*
                var token = await MyFirebaseIIDService.GetToken();

                SaikoroTravelCommon.HttpModels.UserRegisterResponse result = await APIService.PostUserToken(this, new SaikoroTravelCommon.HttpModels.UserRegisterRequest()
                {
                    User = item,
                    Code = code,
                    Token = token,
                });
                */

                var result = new SaikoroTravelCommon.HttpModels.UserRegisterResponse()
                {
                    IsOK = true,
                    Message = ""
                };

                if (result == null)
                {
                    _ = new AlertDialog.Builder(this)
                        .SetTitle("通信エラー")
                        .SetMessage("通信エラーが発生しました。通信状況を確認してください。")
                        .SetPositiveButton("OK", (sender, e) => { })
                        .Show();
                }
                else
                {
                    if (!result.IsOK)
                    {
                        _ = new AlertDialog.Builder(this)
                       .SetTitle("登録エラー")
                       .SetMessage("登録に失敗しました。確認コードが正しいか確かめてください")
                       .SetPositiveButton("OK", (sender, e) => { })
                       .Show();
                    }
                    else
                    {
                        DependencyProvider.Default.RequestConfigManager(this).User = item;
                        SetResult(Result.Ok);
                        Finish();
                    }
                }

                button.Enabled = true;
            };
        }
    }
}