
// **********************************************************************
//
// Copyright (c) 2003-2018 ZeroC, Inc. All rights reserved.
//
// **********************************************************************
#pragma once

module Sample
{
	module Protocol
	{
		struct RLResultRes
		{
			byte ret;
			string idcode;
		};

		["amd"]interface IPlayerCo
		{
		   RLResultRes RegOrLoginReq(string account, string pwd);

		   bool JoinSceneReq(byte sceneid);
		}   	
	}
}