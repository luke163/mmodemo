
// **********************************************************************
//
// Copyright (c) 2003-2018 ZeroC, Inc. All rights reserved.
//
// **********************************************************************
#pragma once

module FootStone
{
	module FrontIce
	{
		interface ISessionPush
		{
			void SessionDestroyed();
		}
   
		interface ISessionFactory
		{		   
			void CreateSession(ISessionPush* proxy);

			void Shutdown();
		}
	}
}