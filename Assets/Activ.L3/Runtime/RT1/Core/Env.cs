using System.Collections.Generic;
using InvOp = System.InvalidOperationException;
using Activ.Util; using L3;

namespace R1{
public partial class Env{

    Stack<CallFrame> store = new (); R1.Obj @object;

    public object pose
    { get => frame.pose; set => frame.pose = value; }

    public void Enter(object pose){
        store.Clear();
        store.Push(new CallFrame( pose ));
        @object = null;
    }

    public void Exit() => store.Pop();

    public void EnterScope() => frame.Push( new () );

    public void ExitScope() => frame.Pop();

    public void EnterCall(Scope arg, object target)
    => store.Push( new CallFrame(arg, target as R1.Obj) );

    public void ExitCall(){ store.Pop(); @object = null; }

    // ------------------------------------------------------------

    public void Def(Node arg) => local.Add(arg);

    // ------------------------------------------------------------

    public Class FindType(string name) => (Class) Find(name);

    public object GetVariableValue(string @var, bool opt)
    => frame.GetValue(@var, opt);

    public object Reference(string @var, bool opt)
    => frame.Reference(@var, opt);

    // -------------------------------------------------------------

    public void Dump(){
        var str = "";
        var i = 0; foreach(var frame in store){
            str += $"FRAME #{i++}\n";
            Dump(frame);
        }
        void Dump(Stack<Scope> frame){
            var j = 0; foreach(var scope in frame){
                str += $"-- SCOPE #{j++}";
                var nodes = scope._nodes;
                if(nodes == null || nodes.Count == 0){
                    str += " (empty)";
                }else foreach(var node in scope._nodes)
                    str += $"\n---- {node}";
            }
        }
        UnityEngine.Debug.Log("STORE\n" + str);
    }

    // TODO remove if possible
    Node Find(string name){
        foreach(var z in frame){
            var output = z.Find(name);
            if(output != null) return output;
        }
        return @object?.Find(name);
    }

    CallFrame frame => store.Peek();

    Scope local => frame.Peek();

}}
