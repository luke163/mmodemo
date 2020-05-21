using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FootStone.NetworkNew;
using Sample.Protocol;

public class MainMenu : MonoBehaviour
{
    public Text acctTxt;
	public Text pswdTxt;
	public Button lognBut;
	
    private float confineTime; //限制每帧执行调用次数

	protected void Awake()
	{
		www.Init();
	}
	
	protected void Start()
	{
		var con = www.Connect("127.0.0.1", 4061);
		con.ContinueWith( (t) => {
			www.logger.Debug($"网络连接没成功啊！{t.Result}");
			if(!t.Result) {
				lognBut.enabled = false;
			}
		} );
	}
		
    public async void LoginAccountSelectMenu()
    {
		var playerprx = www.session.UncheckedCast(IPlayerCoPrxHelper.uncheckedCast, IPlayerCoPrxHelper.ice_staticId());
		var ret = await playerprx.RegOrLoginReqAsync("test", "456789");
    }

    // Update is called once per frame
    void Update()
    {
        confineTime += Time.deltaTime;
        if (confineTime < 0.03f) return;
        
        confineTime = 0;
    }
}
