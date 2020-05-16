
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
		["amd"]interface IAccountPo
		{
		   byte LoginRequest(string account, string pwd);
		}   	
	}
}