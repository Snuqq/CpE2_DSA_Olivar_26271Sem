using System;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using CpE2_DSA_Olivar_26271Sem;

// Intercept dialogs only in this test executable so error paths run unattended.
namespace CpE2_DSA_Olivar_26271Sem
{
    static class MessageBox
    {
        public static string Last;
        public static DialogResult Show(string message) { Last = message; return DialogResult.OK; }
    }
}
class FormChecks
{
    static BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
    static T Field<T>(object form, string name) { return (T)form.GetType().GetField(name, flags).GetValue(form); }
    static void Call(object form, string method) { form.GetType().GetMethod(method, flags).Invoke(form, new object[] { null, EventArgs.Empty }); }
    static void Click(object form, string name) { typeof(Button).GetMethod("OnClick", flags).Invoke(Field<Button>(form, name), new object[] { EventArgs.Empty }); }
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Input(object form, string name, string value) { Field<TextBox>(form, name).Text = value; }
    static void FocusWired(object form, string button)
    {
        var events = (EventHandlerList)typeof(Component).GetProperty("Events", flags).GetValue(Field<Button>(form, button), null);
        var key = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        bool found = false;
        foreach (var handler in events[key].GetInvocationList()) if (handler.Method.Name == "FocusInput") found = true;
        Check(found, button + " has no input-focus handler");
    }
    [STAThread] static int Main()
    {
        int failures = 0;
        foreach (Action test in new Action[] { ArrayChecks, LinkedChecks, StackChecks, QueueChecks, NavigationChecks, SearchPasteChecks, InfographicChecks, NumberGeneratorChecks })
        {
            try { test(); Console.WriteLine("PASS: " + test.Method.Name); }
            catch (Exception e) { failures++; Console.WriteLine("FAIL: " + test.Method.Name + ": " + e.GetBaseException().Message); }
        }
        return failures == 0 ? 0 : 1;
    }
    static void SearchPasteChecks()
    {
        foreach (var form in new Form[] { new JumpSearchForm(), new ExponentialSearchForm(), new FibonacciSearchForm() })
        using (form)
        {
            var input = Field<TextBox>(form, "txtNumbers");
            input.Text = "-3\r\n0\t5\n9";
            Check(input.Text == "-3 0 5 9", form.Name + " lost pasted spreadsheet values");
            input.Text = "1, 3 5,7";
            Check(input.Text == "1, 3 5,7", form.Name + " changed mixed separators");
            input.Clear();
            Check(input.Text.Length == 0, form.Name + " could not clear pasted values");
        }
    }
    static void ArrayChecks()
    {
        using (var form = new ArrayControl())
        {
            var list = Field<ListBox>(form, "lstbArray");
            Click(form, "btnDisplayAll"); Check(list.Items.Count == 4, "Initial array");
            Input(form, "txtValue", "new"); Click(form, "btnInsert"); Check(list.Items.Count == 5 && (string)list.Items[4] == "new", "Append");
            foreach (var bad in new[] { "", "abc", "-1", "5", "9999999999999" })
            { Input(form, "txtIndexNo", bad); Click(form, "btnDisplayIndexValue"); Check(Field<Label>(form, "lblStatus").Text.StartsWith("Enter an index"), "Index validation"); }
            Input(form, "txtIndexNo", "4"); Click(form, "btnDisplayIndexValue"); Check(list.Items.Count == 1 && (string)list.Items[0] == "new", "Indexed display");
            Input(form, "txtValue", " "); Click(form, "btnInsert");
            Click(form, "btnClear"); Check(list.Items.Count == 0, "Clear display");
            Click(form, "btnDisplayAll"); Check(list.Items.Count == 5, "Clear preserves array");
        }
    }
    static void LinkedChecks()
    {
        using (var form = new Linkedlist())
        {
            Call(form, "Linkedlist_Load");
            var items = Field<System.Collections.Generic.LinkedList<int>>(form, "myLinkedList");
            Check(items.Count == 10, "Initial list");
            Field<ComboBox>(form, "Llist").Text = "AddFirst"; Input(form, "txtValue", "42"); Click(form, "bttnInsert"); Check(items.First.Value == 42, "AddFirst");
            Field<ComboBox>(form, "Llist").Text = "AddLast"; Click(form, "bttnInsert"); Check(items.Last.Value == 42, "AddLast");
            Field<ComboBox>(form, "cmbAddBeforeAfter").Text = "AddBefore"; Input(form, "txtCurrent", "1"); Input(form, "txtValue2", "21"); Click(form, "btnInsertBeforeAfter"); Check(items.Find(1).Previous.Value == 21, "AddBefore");
            Field<ComboBox>(form, "cmbAddBeforeAfter").Text = "AddAfter"; Click(form, "btnInsertBeforeAfter"); Check(items.Find(1).Next.Value == 21, "AddAfter");
            foreach (var bad in new[] { "", "abc", "9999999999999", "98765" })
            { int count = items.Count; Input(form, "txtCurrent", bad); Click(form, "btnInsertBeforeAfter"); Check(items.Count == count, "Invalid current mutated list"); }
            Input(form, "txtFind", "42"); Click(form, "btnFind"); Check(CpE2_DSA_Olivar_26271Sem.MessageBox.Last == "Found: 42", "Find");
            Input(form, "txtFind", "98765"); Click(form, "btnFind"); Check(CpE2_DSA_Olivar_26271Sem.MessageBox.Last == "Value not found", "Missing find");
            Field<ComboBox>(form, "cmdRemove").Text = "RemoveFirst"; Click(form, "btnRemove"); Check(items.First.Value == 21, "RemoveFirst");
            Field<ComboBox>(form, "cmdRemove").Text = "RemoveLast"; Click(form, "btnRemove"); Check(items.Last.Value == 10, "RemoveLast");
            Click(form, "btnCount"); Check(CpE2_DSA_Olivar_26271Sem.MessageBox.Last == "Count: " + items.Count, "Count");
            Click(form, "btnClear"); Click(form, "btnRemove"); Check(items.Count == 0, "Empty remove");
            Click(form, "btnReset"); Check(items.Count == 10, "Reset");
            Click(form, "bttndisplay"); Check(Field<ListBox>(form, "lstbLinkedList").Items.Count == 10, "Display");
        }
    }
    static void StackChecks() { using(var form = new CpE2_DSA_Olivar_26271Sem.Stack()) CollectionChecks(form, "Stack", "btnpush", "btnpop"); }
    static void QueueChecks() { using(var form = new CpE2_DSA_Olivar_26271Sem.Queue()) CollectionChecks(form, "Queue", "btnEnqueue", "btnDequeue"); }
    static void CollectionChecks(Form form, string kind, string add, string remove)
    {
        var list = Field<ListBox>(form, "lstDisplay");
        Click(form, remove); Click(form, "btnPeek");
        Input(form, "txt" + kind, " "); Click(form, add); Check(list.Items.Count == 0, "Blank input");
        foreach(var value in new[] { "a", "b", "c" }) { Input(form, "txt" + kind, value); Click(form, add); }
        Check(list.Items.Count == 3, "Add");
        Check((string)list.Items[0] == (kind == "Stack" ? "c" : "a"), "Order");
        Click(form, "btnPeek"); Check(list.Items.Count == 3, "Peek mutation");
        Click(form, remove); Check((string)list.Items[0] == "b" && list.Items.Count == 2, "Removal");
        Input(form, "txt" + kind, "b"); Click(form, "btnContains"); Check(Field<Label>(form, "lblStatus").Text.EndsWith("True"), "Contains");
        Input(form, "txt" + kind, "missing"); Click(form, "btnContains"); Check(Field<Label>(form, "lblStatus").Text.EndsWith("False"), "Missing contains");
        Click(form, "btnCount"); Check(Field<Label>(form, "lblStatus").Text == "Count: 2", "Count");
        Click(form, "btnClear"); Check(list.Items.Count == 0, "Clear");
        foreach(var button in new[] { add, remove, "btnContains", "btnCount", "btnClear", "btnPeek" }) FocusWired(form, button);
    }
    static void NumberGeneratorChecks()
    {
        foreach (var form in new Form[] { new JumpSearchForm(), new ExponentialSearchForm(), new FibonacciSearchForm() })
        using (form)
        {
            var count = Field<NumericUpDown>(form, "numberCount");
            var numbers = Field<TextBox>(form, "txtNumbers");
            var target = Field<TextBox>(form, "txtTarget");
            Check(count.Minimum == 1 && count.Maximum == 40 && count.Value == 10, "Generator count limits");
            foreach (int amount in new[] { 1, 10, 40 })
            {
                count.Value = amount;
                Click(form, "btnGenerate");
                string[] tokens = numbers.Text.Split(',');
                Check(tokens.Length == amount, "Generated count");
                int[] values = new int[amount];
                for (int i = 0; i < amount; i++)
                {
                    Check(int.TryParse(tokens[i].Trim(), out values[i]), "Generated invalid integer");
                    Check(values[i] > 0 && (i == 0 || values[i] > values[i - 1]), "Generated values not sorted and distinct");
                }
                int wanted = int.Parse(target.Text);
                Check(System.Array.IndexOf(values, wanted) >= 0, "Generated target missing");
                foreach (int searchTarget in new[] { wanted, 0 })
                {
                    var steps = (System.Collections.Generic.List<SearchStep>)form.GetType().GetMethod("CreateSteps").Invoke(null, new object[] { values, searchTarget });
                    var result = steps[steps.Count - 1];
                    Check(result.Terminal && result.Index == System.Array.IndexOf(values, searchTarget), "Generated search result");
                    var plan = new SearchPlan { Values = values, Target = searchTarget, Algorithm = Field<UnitySearchHost>(form, "pnlAnimation").Algorithm, Steps = steps };
                    Check(plan.Serialize().Length > 0, "Generated plan cannot be sent to Unity");
                }
            }
            string before = numbers.Text;
            form.GetType().GetField("loaded", flags).SetValue(form, true);
            Click(form, "btnGenerate");
            Check(numbers.Text == before, "Generator changed a loaded run");
            count.Enabled = Field<Button>(form, "btnGenerate").Enabled = false;
            Click(form, "btnReset");
            Check(count.Enabled && Field<Button>(form, "btnGenerate").Enabled, "Reset did not unlock generator");
            count.Value = 1;
            Click(form, "btnGenerate");
            Check(numbers.Text.IndexOf(',') < 0, "Generate failed after reset");
        }
    }

    static void InfographicChecks()
    {
        using (var main = new Form1())
        {
            main.WindowState = FormWindowState.Normal;
            main.Show();
            foreach (var size in new[] { new System.Drawing.Size(1284, 941), new System.Drawing.Size(980, 570) })
            {
                main.Size = size;
                Click(main, "btnInfographic");
                Application.DoEvents();
                var form = Field<SplitContainer>(main, "splitContainer2").Panel2.Controls[0];
                var tabs = (TabControl)form.Controls.Find("infographicTabs", true)[0];
                Check(tabs.TabPages.Count == 4, "Infographic tabs");
                CheckInfographicLabels(tabs.SelectedTab);
                int[] counts = { 4, 6, 4 };
                string[] offsets = { "Offset -1 → 4", "Offset 4 → 4", "Offset 4 → 5", "Offset 5 → 5" };
                for (int tab = 1; tab < tabs.TabPages.Count; tab++)
                {
                    tabs.SelectedIndex = tab;
                    Application.DoEvents();
                    CheckInfographicLabels(tabs.SelectedTab);
                    var about = tabs.SelectedTab.Controls.Find("aboutAlgorithm", true)[0];
                    Check(about.Visible, "About view should open first");
                    string lesson = "";
                    foreach (Control child in InfographicControls(about)) lesson += child.Text + "\n";
                    Check(lesson.Contains("Requires sorted data") && lesson.Contains("Worst-case search time"), "Missing educational requirements");
                    Check(lesson.Contains(tab == 1 ? "Ben Shneiderman" : tab == 2 ? "Andrew Chi-Chih Yao" : "David E. Ferguson"), "Missing historical credit");
                    if (tab == 1) Check(lesson.Contains("does not establish a single original inventor"), "Jump history overstates attribution");
                    ((Button)tabs.SelectedTab.Controls.Find("btnHistory", true)[0]).PerformClick();
                    Application.DoEvents();
                    Check(tabs.SelectedTab.Controls.Find("historyLesson", true)[0].Visible, "History page did not open");
                    CheckInfographicLabels(tabs.SelectedTab);
                    ((Button)tabs.SelectedTab.Controls.Find("btnHistoryExample", true)[0]).PerformClick();
                    Application.DoEvents();
                    Check(!about.Visible, "History example did not open");
                    var historyReturn = (LinkLabel)tabs.SelectedTab.Controls.Find("lnkAbout", true)[0];
                    typeof(LinkLabel).GetMethod("OnLinkClicked", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(historyReturn, new object[] { new LinkLabelLinkClickedEventArgs(historyReturn.Links[0]) });
                    Application.DoEvents();
                    Check(tabs.SelectedTab.Controls.Find("historyLesson", true)[0].Visible, "History view was lost");
                    ((Button)tabs.SelectedTab.Controls.Find("btnBasics", true)[0]).PerformClick();
                    Application.DoEvents();
                    ((Button)tabs.SelectedTab.Controls.Find("btnTryExample", true)[0]).PerformClick();
                    Application.DoEvents();
                    Check(!about.Visible && tabs.SelectedTab.Controls.Find("walkthrough", true)[0].Visible, "Example did not open");
                    Button next = null, previous = null;
                    foreach (Control child in InfographicControls(tabs.SelectedTab))
                        if (child is Button) { if (child.Text == "Next") next = (Button)child; else if (child.Text == "Previous") previous = (Button)child; }
                    Check(next != null && previous != null && !previous.Enabled, "Step controls initial state");
                    for (int step = 0; step < counts[tab - 1]; step++)
                    {
                        CheckInfographicLabels(tabs.SelectedTab);
                        string decision = tabs.SelectedTab.Controls.Find("decision", true)[0].Text;
                        string detail = tabs.SelectedTab.Controls.Find("detail", true)[0].Text;
                        if (tab == 1 && step == 2) Check(decision.Contains("Scan indexes 6–7, starting at 6"), "Jump overshoot explanation");
                        if (tab == 3) Check(detail.StartsWith(offsets[step]), "Fibonacci offset change");
                        if (step == counts[tab - 1] - 1)
                        {
                            Check(!next.Enabled, "Next enabled after found");
                            Check(tabs.SelectedTab.Controls.Find("comparison", true)[0].Text.Contains("Found at index 6"), "Final match explanation");
                        }
                        else { Check(next.Enabled, "Missing next step"); next.PerformClick(); Application.DoEvents(); }
                    }
                    var aboutLink = (LinkLabel)tabs.SelectedTab.Controls.Find("lnkAbout", true)[0];
                    typeof(LinkLabel).GetMethod("OnLinkClicked", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(aboutLink, new object[] { new LinkLabelLinkClickedEventArgs(aboutLink.Links[0]) });
                    Application.DoEvents();
                    Check(about.Visible, "Cannot return to About");
                    ((Button)tabs.SelectedTab.Controls.Find("btnTryExample", true)[0]).PerformClick();
                    Application.DoEvents();
                    Check(!next.Enabled, "About reset the example position");
                    while (previous.Enabled) { previous.PerformClick(); Application.DoEvents(); }
                    Check(next.Enabled, "Cannot replay after going back");
                }
            }
        }
    }

    static System.Collections.Generic.IEnumerable<Control> InfographicControls(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            yield return child;
            foreach (Control nested in InfographicControls(child)) yield return nested;
        }
    }

    static void CheckInfographicLabels(Control parent)
    {
        foreach (Control child in InfographicControls(parent))
        {
            var label = child as Label;
            if (label == null || !label.Visible) continue;
            var measured = TextRenderer.MeasureText(label.Text, label.Font,
                new System.Drawing.Size(label.Width, 1000), TextFormatFlags.WordBreak);
            Check(measured.Height <= label.Height, "Infographic text clipped: " + label.Text);
        }
    }
    static void NavigationChecks()
    {
        using(var form = new Form1())
        {
            var panel = Field<SplitContainer>(form, "splitContainer2").Panel2;
            Control previous = null;
            foreach(var name in new[] { "Array", "LinkedList", "Stack", "Queue", "Array" })
            { Call(form, "btn" + name + "_Click"); Check(panel.Controls.Count == 1, "Screen count"); if(previous != null) Check(previous.IsDisposed, "Old screen disposal"); previous = panel.Controls[0]; }
        }
    }
}

