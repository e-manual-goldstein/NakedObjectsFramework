﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Selenium.Test.ObjectTests;

public abstract class GeminiTest : BaseTest {
    protected string GeminiBaseUrl => BaseUrl + "gemini/";

    protected void AssertElementExists(string cssSelector) {
        Wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count >= 1);
    }

    protected static void WaitUntilElementDoesNotExist(string cssSelector) {
        Wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == 0);
    }

    protected void AssertElementCountIs(string cssSelector, int count) {
        Wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == count);
    }

    protected virtual void OpenMainMenu(string menuName) {
        ClickHomeButton();
        WaitForView(Pane.Single, PaneType.Home, "Home");
        var menuSelector = $"nof-menu-bar nof-action input[title=\"{menuName}\"";
        Wait.Until(dr => dr.FindElement(By.CssSelector(menuSelector)));
        var menu = Driver.FindElement(By.CssSelector($"nof-menu-bar nof-action input[title=\"{menuName}\"]"));
        Click(menu);
    }

    protected void OpenMainMenuAction(string menuName, string actionName) {
        OpenMainMenu(menuName);
        var actionSelector = $"nof-action-list nof-action input[value=\"{actionName}\"]";
        Click(WaitForCss(actionSelector));
    }

    protected IWebElement WaitForTitle(string title, Pane pane = Pane.Single) =>
        WaitForTextEquals(CssSelectorFor(pane) + " .title", title);

    #region Helpers

    protected static void Url(string url, bool trw = false) {
        Driver.Navigate().GoToUrl(url);
    }

    protected void GeminiUrl(string url) {
        Url(GeminiBaseUrl + url);
    }

    protected static void WaitUntilGone<TResult>(Func<IWebDriver, TResult> condition) {
        Wait.Until(d => {
            try {
                condition(d);
                return false;
            }
            catch (NoSuchElementException) {
                return true;
            }
        });
    }

    protected virtual void Maximize() {
        const string script = "window.moveTo(0, 0); window.resizeTo(screen.availWidth, screen.availHeight);";
        ((IJavaScriptExecutor)Driver).ExecuteScript(script);
    }

    protected virtual void ScrollTo(IWebElement element) {
        var actions = new Actions(Driver);
        actions.MoveToElement(element);
        actions.Perform();
    }

    protected virtual void Click(IWebElement element) {
        WaitUntilEnabled(element);
        ScrollTo(element);
        element.Click();
    }

    protected static void WaitUntilEnabled(IWebElement element) {
        Wait.Until(dr => element.GetAttribute("disabled") == null);
    }

    protected virtual void RightClick(IWebElement element) {
        var webDriver = Wait.Driver;
        ScrollTo(element);
        var actions = new Actions(webDriver);
        actions.ContextClick(element);
        actions.Perform();
    }

    protected virtual IWebElement WaitForCss(string cssSelector) {
        return Wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
    }

    protected IWebElement WaitForTextEquals(string cssSelector, string text) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).Text.Trim() == text.Trim());
        return WaitForCss(cssSelector);
    }

    protected IWebElement WaitForTextEquals(string cssSelector, int index, string text) {
        Wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector))[index].Text == text);
        return WaitForCss(cssSelector);
    }

    protected IWebElement WaitForTextStarting(string cssSelector, string startOftext) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).Text.StartsWith(startOftext));
        return WaitForCss(cssSelector);
    }

    /// <summary>
    ///     Waits until there are AT LEAST the specified count of matches & returns ALL matches
    /// </summary>
    protected virtual ReadOnlyCollection<IWebElement> WaitForCss(string cssSelector, int count) {
        Wait.Until(d => d.FindElements(By.CssSelector(cssSelector)).Count >= count);
        return Driver.FindElements(By.CssSelector(cssSelector));
    }

    /// <summary>
    ///     Waits for the Nth match and returns it (counting from zero).
    /// </summary>
    protected virtual IWebElement WaitForCssNo(string cssSelector, int number) => WaitForCss(cssSelector, number + 1)[number];

    protected void WaitForMessage(string message, Pane pane = Pane.Single) {
        var p = CssSelectorFor(pane);
        Wait.Until(dr => dr.FindElement(By.CssSelector(p + ".header .messages")).Text == message);
    }

    protected virtual void ClearFieldThenType(string cssFieldId, string characters) {
        var input = WaitForCss(cssFieldId);
        if (input.GetAttribute("value") != "") {
            input.SendKeys(Keys.Control + "a");
            Thread.Sleep(100);
            input.SendKeys(Keys.Delete);
            Thread.Sleep(100);
            Wait.Until(dr => dr.FindElement(By.CssSelector(cssFieldId)).GetAttribute("value") == "");
        }

        input.SendKeys(characters);
    }

    protected virtual void ClearDateFieldThenType(string cssFieldId, string characters) {
        var input = WaitForCss(cssFieldId);
        if (input.GetAttribute("value") != "") {
            for (var i = 0; i < 3; i++) {
                input.SendKeys(Keys.Delete);
                Thread.Sleep(100);
                input.SendKeys(Keys.Tab);
                Thread.Sleep(100);
            }

            input.SendKeys(Keys.Shift + Keys.Tab);
            input.SendKeys(Keys.Shift + Keys.Tab);
            Wait.Until(dr => dr.FindElement(By.CssSelector(cssFieldId)).GetAttribute("value") == "");
        }

        input.SendKeys(characters);
    }

    protected virtual void TypeIntoFieldWithoutClearing(string cssFieldId, string characters) {
        var input = WaitForCss(cssFieldId);
        input.SendKeys(characters);
    }

    protected static void SelectCheckBox(string css, bool alreadySelected = false) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(css)).Selected == alreadySelected);
        var checkbox = Driver.FindElement(By.CssSelector(css));
        checkbox.Click();
        Wait.Until(dr => dr.FindElement(By.CssSelector(css)).Selected == !alreadySelected);
    }

    /// <summary>
    ///     Returns a string of n backspace keys for typing into a field
    /// </summary>
    protected string Repeat(string keys, int n) {
        var sb = new StringBuilder();
        for (var i = 0; i < n; i++) {
            sb.Append(keys);
        }

        return sb.ToString();
    }

    protected virtual void SelectDropDownOnField(string cssFieldId, string characters) {
        var selected = new SelectElement(WaitForCss(cssFieldId));
        selected.SelectByText(characters);
        Wait.Until(dr => selected.SelectedOption.Text == characters);
    }

    protected virtual void SelectDropDownOnField(string cssFieldId, int index) {
        var selected = new SelectElement(WaitForCss(cssFieldId));
        selected.SelectByIndex(index);
    }

    protected virtual void WaitForMenus() {
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-menu-bar nof-action")).Count == 10);
    }

    protected virtual void GoToMenuFromHomePage(string menuName) {
        WaitForView(Pane.Single, PaneType.Home, "Home");

        WaitForMenus();

        var menus = Driver.FindElements(By.CssSelector("nof-action input"));
        var menu = menus.FirstOrDefault(s => s.GetAttribute("value") == menuName);
        if (menu != null) {
            Click(menu);
            Wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
        }
        else {
            throw new NotFoundException($"menu not found {menuName}");
        }
    }

    protected virtual void OpenObjectActions(Pane pane = Pane.Single) {
        var paneSelector = CssSelectorFor(pane);
        var actions = Wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + "input")).Single(el => el.GetAttribute("value") == "Actions"));
        Click(actions);
        Wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + " nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
    }

    protected virtual void OpenSubMenu(string menuName, Pane pane = Pane.Single) {
        var paneSelector = CssSelectorFor(pane);
        var sub = Wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + " .submenu")).Single(el => el.Text == menuName));
        var expand = sub.FindElement(By.CssSelector(".icon-expand"));
        Click(expand);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".icon-collapse")).Count > 0);
    }

    protected virtual void OpenMenu(string menuName, Pane pane = Pane.Single) {
        var paneSelector = CssSelectorFor(pane);
        var menu = Wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + "input")).Single(el => el.GetAttribute("value") == menuName));
        if (menu != null) {
            Click(menu);
            Wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
        }
        else {
            throw new NotFoundException($"menu not found {menuName}");
        }
    }

    protected virtual void CloseSubMenu(string menuName) {
        var sub = Wait.Until(dr => dr.FindElements(By.CssSelector(".submenu")).Single(el => el.Text == menuName));
        var expand = sub.FindElement(By.CssSelector(".icon-collapse"));
        Click(expand);
        Assert.IsNotNull(sub.FindElement(By.CssSelector(".icon-expand")));
    }

    protected void Login() {
        Thread.Sleep(2000);
    }

    #endregion

    #region Resulting page view

    protected enum Pane {
        Single,
        Left,
        Right
    }

    protected enum PaneType {
        Home,
        Object,
        List,
        Recent,
        MultiLineDialog,
        Attachment,
        ApplicationProperties,
        Error,
        Logoff
    }

    protected enum ClickType {
        Left,
        Right
    }

    protected string GetPropertyValue(string propertyName, Pane pane = Pane.Single) {
        var prop = GetProperty(propertyName, pane);
        return prop.FindElement(By.CssSelector(".value")).Text.Trim();
    }

    protected IWebElement GetProperty(string propertyName, Pane pane = Pane.Single) {
        var propCss = CssSelectorFor(pane) + " " + "nof-view-property";
        return Wait.Until(dr => dr.FindElements(By.CssSelector(propCss)).Single(we => we.FindElement(By.CssSelector(".name")).Text == propertyName + ":"));
    }

    protected IWebElement GetReferenceFromProperty(string propertyName, Pane pane = Pane.Single) {
        var prop = GetProperty(propertyName, pane);
        return prop.FindElement(By.CssSelector(".reference"));
    }

    protected IWebElement GetReferenceProperty(string propertyName, string refTitle, Pane pane = Pane.Single) {
        var propCss = CssSelectorFor(pane) + " " + ".property";
        var prop = Wait.Until(dr => dr.FindElements(By.CssSelector(propCss)).Single(we => we.FindElement(By.CssSelector(".name")).Text == propertyName + ":" &&
                                                                                          we.FindElement(By.CssSelector(".reference")).Text == refTitle)
        );
        return prop.FindElement(By.CssSelector(".reference"));
    }

    protected static string CssSelectorFor(Pane pane) =>
        pane switch {
            Pane.Single => ".single ",
            Pane.Left => "#pane1 ",
            Pane.Right => "#pane2 ",
            _ => throw new NotImplementedException()
        };

    protected virtual void WaitForView(Pane pane, PaneType type, string title = null) {
        var selector = CssSelectorFor(pane) + " ." + type.ToString().ToLower();

        if (title != null) {
            selector += " .header .title";
            Wait.Until(dr => dr.FindElement(By.CssSelector(selector)).Text == title);
        }
        else {
            WaitForCss(selector);
        }

        WaitUntilElementDoesNotExist(pane == Pane.Single ? ".split" : ".single");

        AssertFooterExists();
    }

    protected virtual void AssertFooterExists() {
        Wait.Until(d => d.FindElement(By.CssSelector(".footer")));
        Wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.home")).Displayed);
        Wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.back")).Displayed);
        Wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.forward")).Displayed);
    }

    protected void AssertTopItemInListIs(string title) {
        var topItem = WaitForCss("tr td.reference").Text;

        Assert.AreEqual(title, topItem);
    }

    #endregion

    #region Editing & Saving

    protected void EditObject() {
        Click(EditButton());
        SaveButton();
        GetCancelEditButton();
        var title = Driver.FindElement(By.CssSelector(".header .title")).Text;
        Assert.IsTrue(title.StartsWith("Editing"));
    }

    protected void SaveObject(Pane pane = Pane.Single) {
        Click(SaveButton(pane));
        EditButton(pane); //To Wait for save completed
        var title = Driver.FindElement(By.CssSelector(".header .title")).Text;
        Assert.IsFalse(title.StartsWith("Editing"));
    }

    protected void CancelObject(Pane pane = Pane.Single) {
        Click(GetCancelEditButton(pane));
        EditButton(pane); //To Wait for cancel completed
        var title = Driver.FindElement(By.CssSelector(".header .title")).Text;
        Assert.IsFalse(title.StartsWith("Editing"));
    }

    protected IWebElement GetButton(string text, Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + ".header .action";
        return Wait.Until(dr => dr.FindElements(By.CssSelector(selector)).Single(e => e.Text == text));
    }

    protected IWebElement GetInputButton(string text, Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + "input";
        return Wait.Until(dr => dr.FindElements(By.CssSelector(selector)).Single(e => e.GetAttribute("value") == text));
    }

    protected IWebElement EditButton(Pane pane = Pane.Single) => GetInputButton("Edit", pane);

    protected IWebElement SaveButton(Pane pane = Pane.Single) => GetInputButton("Save", pane);

    protected IWebElement SaveVMButton(Pane pane = Pane.Single) => GetInputButton("Save", pane);

    protected IWebElement SaveAndCloseButton(Pane pane = Pane.Single) => GetInputButton("Save & Close", pane);

    protected IWebElement GetCancelEditButton(Pane pane = Pane.Single) =>
        GetInputButton("Cancel", pane);

    protected void ClickHomeButton() {
        Click(WaitForCss(".icon.home"));
    }

    protected void ClickBackButton() {
        Click(WaitForCss(".icon.back"));
    }

    protected void ClickForwardButton() {
        Click(WaitForCss(".icon.forward"));
    }

    protected void ClickRecentButton() {
        Click(WaitForCss(".icon.recent"));
    }

    protected void ClickPropertiesButton() {
        Click(WaitForCss(".icon.properties"));
    }

    protected void ClickLogOffButton() {
        Click(WaitForCss(".icon.logoff"));
    }

    #endregion

    #region Object Actions

    protected ReadOnlyCollection<IWebElement> GetObjectActions(int totalNumber, Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + "nof-action-list nof-action > input";
        Wait.Until(d => d.FindElements(By.CssSelector(selector)).Count == totalNumber);
        return Driver.FindElements(By.CssSelector(selector));
    }

    protected static void AssertAction(int number, string actionName) {
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-action-list nof-action > input"))[number].GetAttribute("value") == actionName);
    }

    protected virtual void AssertActionNotDisplayed(string action) {
        Wait.Until(dr => dr.FindElements(By.CssSelector($"nof-action-list nof-action inputinput[type='{action}']")).FirstOrDefault() == null);
    }

    protected IWebElement GetObjectAction(string actionName, Pane pane = Pane.Single, string subMenuName = null) {
        if (subMenuName != null) {
            OpenSubMenu(subMenuName);
        }

        var selector = CssSelectorFor(pane) + $"nof-action-list nof-action input[value='{actionName}']";
        return Wait.Until(d => d.FindElement(By.CssSelector(selector)));
    }

    protected IWebElement GetLCA(string actionName, Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + $"nof-collection nof-action input[value='{actionName}']";
        return Wait.Until(d => d.FindElement(By.CssSelector(selector)));
    }

    protected IWebElement GetObjectEnabledAction(string actionName, Pane pane = Pane.Single, string subMenuName = null) {
        var a = GetObjectAction(actionName, pane, subMenuName);

        if (a.Enabled) {
            return a;
        }

        Thread.Sleep(1000);

        a = GetObjectAction(actionName, pane, subMenuName);

        if (a.Enabled) {
            return a;
        }

        throw new Exception("Action not enabled");
    }

    protected IWebElement OpenActionDialog(string actionName, Pane pane = Pane.Single, int? noOfParams = null) {
        Click(GetObjectEnabledAction(actionName, pane));

        var dialogSelector = CssSelectorFor(pane) + " .dialog ";
        Wait.Until(d => d.FindElement(By.CssSelector(dialogSelector + "> .title")).Text == actionName);
        //Check it has OK & cancel buttons
        Wait.Until(d => Driver.FindElement(By.CssSelector(dialogSelector + ".ok")));
        Wait.Until(d => Driver.FindElement(By.CssSelector(dialogSelector + ".cancel")));
        //Wait for params if required
        if (noOfParams != null) {
            Wait.Until(dr => dr.FindElements(By.CssSelector(dialogSelector + " .parameter")).Count == noOfParams.Value);
        }

        return WaitForCss(dialogSelector);
    }

    protected static IWebElement GetInputNumber(IWebElement dialog, int no) {
        Wait.Until(dr => dialog.FindElements(By.CssSelector(".parameter .value input")).Count >= no + 1);
        return dialog.FindElements(By.CssSelector(".parameter .value input"))[no];
    }

    protected IWebElement OKButton() => WaitForCss(".dialog .ok");

    //For use with multi-line dialogs, lineNo starts from zero
    protected static IWebElement OKButtonOnLine(int lineNo) {
        return Wait.Until(dr => dr.FindElements(By.CssSelector(".lineDialog"))[lineNo].FindElement(By.CssSelector(".ok")));
    }

    protected void WaitForOKButtonToDisappear(int lineNo) {
        var line = WaitForCssNo(".lineDialog", lineNo);
        Wait.Until(dr => line.FindElements(By.CssSelector(".ok")).Count == 0);
    }

    protected void WaitForReadOnlyEnteredParam(int lineNo, int paramNo, string value) {
        var line = WaitForCssNo(".lineDialog", lineNo);

        Wait.Until(dr => line.FindElements(By.CssSelector(".parameter .value"))[paramNo].Text == value);
    }

    protected void CancelDialog(Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + ".dialog ";
        Click(WaitForCss(selector + ".cancel"));

        Wait.Until(dr => {
            try {
                dr.FindElement(By.CssSelector(selector));
                return false;
            }
            catch (NoSuchElementException) {
                return true;
            }
        });
    }

    protected void AssertHasFocus(IWebElement el) {
        Wait.Until(dr => dr.SwitchTo().ActiveElement() == el);
    }

    protected void Reload(Pane pane = Pane.Single) {
        Click(GetInputButton("Reload", pane));
    }

    protected void CancelDatePicker(string cssForInput) {
        var dp = Driver.FindElement(By.CssSelector(".ui-datepicker"));
        if (dp.Displayed) {
            WaitForCss(cssForInput).SendKeys(Keys.Escape);
            Wait.Until(br => !br.FindElement(By.CssSelector(".ui-datepicker")).Displayed);
        }
    }

    protected static void PageDownAndWait() {
        Driver.SwitchTo().ActiveElement().SendKeys(Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown);
        Thread.Sleep(1000);
    }

    #endregion

    #region CCAs

    protected void CheckIndividualItem(int itemNo, string label, string value, bool equal = true) {
        GeminiUrl("object?o1=___1.SpecialOffer--" + (itemNo + 1));
        var html = label + "\r\n" + value;
        if (equal) {
            //Thread.Sleep(2000);

            //var t = WebDriver.FindElements(By.CssSelector(".property")).First().Text;

            Wait.Until(dr => dr.FindElements(By.CssSelector(".property")).First(p => p.Text.StartsWith(label)).Text == html);
        }
        else {
            Wait.Until(dr => dr.FindElements(By.CssSelector(".property")).First(p => p.Text.StartsWith(label)).Text != html);
        }
    }

    protected void WaitForSelectedCheckboxes(int number) => Wait.Until(dr => dr.FindElements(By.CssSelector("input[type='checkbox']")).Count(el => el.Selected && el.Enabled) == number);

    protected void WaitForSelectedCheckboxesAtLeast(int number) => Wait.Until(dr => dr.FindElements(By.CssSelector("input[type='checkbox']")).Count(el => el.Selected && el.Enabled) >= number);

    #endregion

    #region ToolBar icons

    protected IWebElement HomeIcon() => WaitForCss(".footer .icon.home");

    protected IWebElement SwapIcon() => WaitForCss(".footer .icon.swap");

    protected IWebElement FullIcon() => WaitForCss(".footer .icon.full");

    protected void GoBack(int clicks = 1) {
        for (var i = 1; i <= clicks; i++) {
            Click(Driver.FindElement(By.CssSelector(".icon.back")));
        }
    }

    #endregion

    #region Keyboard navigation

    protected void CopyToClipboard(IWebElement element) {
        var title = element.Text;
        element.SendKeys(Keys.Control + "c");
        Wait.Until(dr => dr.FindElement(By.CssSelector(".footer .currentcopy .reference")).Text == title);
    }

    protected IWebElement PasteIntoInputField(string cssSelector) {
        var target = WaitForCss(cssSelector);
        var copying = WaitForCss(".footer .currentcopy .reference").Text;
        target.Click();
        target.SendKeys(Keys.Control + "v");
        Wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).GetAttribute("value") == copying);
        return WaitForCss(cssSelector);
    }

    protected IWebElement PasteIntoReferenceField(string cssSelector) {
        var target = WaitForCss(cssSelector);
        var copying = WaitForCss(".footer .currentcopy .reference").Text;
        target.Click();
        target.SendKeys(Keys.Control + "v");
        Wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).GetAttribute("value") == copying);
        return WaitForCss(cssSelector);
    }

    protected IWebElement Tab(int numberIfTabs = 1) {
        for (var i = 1; i <= numberIfTabs; i++) {
            Driver.SwitchTo().ActiveElement().SendKeys(Keys.Tab);
        }

        return Driver.SwitchTo().ActiveElement();
    }

    #endregion

    #region Cicero helper methods

    protected void CiceroUrl(string url) {
        Driver.Navigate().GoToUrl(TestConfig.BaseObjectUrl + "cicero/" + url);
    }

    protected void WaitForOutput(string output) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(".output")).Text == output);
    }

    protected void WaitForOutputStarting(string output) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(".output")).Text.StartsWith(output));
    }

    protected void WaitForOutputContaining(string output) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(".output")).Text.Contains(output));
    }

    protected void EnterCommand(string command) {
        Wait.Until(dr => dr.FindElement(By.CssSelector("input")).Text == "");
        TypeIntoFieldWithoutClearing("input", command);
        Thread.Sleep(300); //To make it easier to see that the command has been entered
        TypeIntoFieldWithoutClearing("input", Keys.Enter);
    }

    #endregion
}

public static class ExtensionMethods {
    public static IWebElement AssertIsDisabled(this IWebElement a, string reason = null) {
        Assert.IsNotNull(a.GetAttribute("disabled"), "Element " + a.Text + " is not disabled");
        if (reason != null) {
            Assert.AreEqual(reason, a.GetAttribute("title"));
        }

        return a;
    }

    public static IWebElement AssertIsEnabled(this IWebElement a) {
        Assert.IsNull(a.GetAttribute("disabled"), "Element " + a.Text + " is disabled");
        return a;
    }

    public static IWebElement AssertHasTooltip(this IWebElement a, string tooltip) {
        Assert.AreEqual(tooltip, a.GetAttribute("title"));
        return a;
    }

    public static IWebElement AssertIsInvisible(this IWebElement a) {
        Assert.IsNull(a.GetAttribute("displayed"));
        return a;
    }
}