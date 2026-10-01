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
        foreach (Action test in new Action[] { ArrayChecks, LinkedChecks, StackChecks, QueueChecks, NavigationChecks })
        {
            try { test(); Console.WriteLine("PASS: " + test.Method.Name); }
            catch (Exception e) { failures++; Console.WriteLine("FAIL: " + test.Method.Name + ": " + e.GetBaseException().Message); }
        }
        return failures == 0 ? 0 : 1;
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
        Click(form, "btnClear"); Click(form, "btnDisplay"); Check(list.Items.Count == 0, "Clear");
        foreach(var button in new[] { add, remove, "btnDisplay", "btnContains", "btnCount", "btnClear", "btnPeek" }) FocusWired(form, button);
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

