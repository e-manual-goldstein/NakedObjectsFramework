﻿using NakedFramework.RATL.Classic.Interface;
namespace NakedFramework.RATL.Classic.Interface;
public interface ITestMenu {

    string Title { get; }

    string MenuId { get; }

    ITestMenu AssertNameEquals(string name);
    ITestMenu AssertItemCountIs(int count);
    ITestAction GetAction(string name);
    ITestMenu GetSubMenu(string name);
    ITestMenuItem GetItem(string name);
    ITestMenuItem[] AllItems();
}