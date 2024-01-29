using System.Reflection;
using InvOp = System.InvalidOperationException;
using L3;
using System.Linq;
using UnityEngine;

namespace R1{
public static class New{

    // NOTE - target may be null
    public static object Invoke(
        L3.New nw, Scope scope, Context cx, object target
    ){
        cx.Log("call/" + nw);
        var name = nw.type;
        // Find the wanted function,
        var node = scope?.Find(name);
        ConstructorInfo[] cs = null;
        if(node == null){
            cs = ResolveCsConstructor(name, target ?? cx, nw.args.Count);
            if(cs == null){
                if(nw.opt){
                    return false;
                }else throw new InvOp(
                    $"No func or C# method matching {name}"
                );
            }
        }
        // Resolve args to sub-scope
        var sub = new Scope();
        foreach(var arg in nw.args){
            sub.Add(cx.Step(arg as Node) as Node);
        }
        if(cs != null && cs.Length > 0){  // (C#) native call
            return CSharp.Construct(cs, sub, target ?? cx);
        }else{                            // L3 call
            if(node == null) throw new InvOp($"No constructor for {nw.type}");
            // Push the subscope
            cx.stack.Push( sub );
            Debug.Log($"CALL l3 constructor: [{node}]");
            var output = cx.Instantiate(node as Class);
            // Exit subscope and return the output
            cx.stack.Pop();
            return output;
        }
    }

    static ConstructorInfo[] ResolveCsConstructor(
        string name, object context, int count
    ) => context.GetType().GetConstructors()
                        .Where(m => m.Name == name)
                        .Where(m => m.GetParameters().Length == count)
                        .ToArray();

}}