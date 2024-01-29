using System.Reflection;
using L3; using Co = L3.Composite;
using static L3.Composite.Type;
using static L3.Token;
using InvOp = System.InvalidOperationException;
using UnityEngine;

namespace R1{
public static class Composite{

    // TODO BT style composites not implemented
    public static object Step(Co co, Context cx)
    => co.type switch{
        access => Access(co, cx),
        act => Act(co, cx),
        assign => Assign(co, cx),
        block => Block(co, cx),
        sel => Sel(co, cx),
        seq => Seq(co, cx),
        _ => throw new InvOp($"Unknown composite: {co.type}")
    };

    public static object Block(Co co, Context cx){
        cx.Log("blk/" + co);
        foreach(var k in co.nodes) cx.Step(k as Node);
        return null;
    }

    public static object Access(Co co, Context cx){
        cx.Log("access/" + co);
        object prev = null, val = null;
        foreach(var k in co.nodes){
            if(prev != null){
                Debug.Log($"Access {prev}");
                val = ((Accessible)prev).Find(k as Node, cx);
                prev = val;
            }else{
                val = cx.Step(k as Node);
                prev = val;
            }
        }
        return val;
    }

    public static object Assign(Co co, Context cx){
        cx.Log("assign/" + co);
        var n = co.nodes.Count;
        Node val = null;
        for(int i = n - 1; i >= 0; i--){
            var next = cx.Step(co.nodes[i] as Node);
            if(val != null){
                Assign((Node)val, (Node)next);
            }
            val = (Node) next;
        }
        return val;
        void Assign(Node value, Node @var){
            UnityEngine.Debug.Log($"assign {value} to {@var}");
            var assignable = (Assignable)@var;
            assignable.Assign(value);
        }
    }

    public static object Sel(Co co, Context cx){
        cx.Log("sel/" + co);
        object val = null;
        foreach(var k in co.nodes){
            val = cx.Step(k as Node);
            if(val == (object)true) return val;
            if(val == (object)@cont) return val;
            if(val == (object)@void) return val;
        }
        return val;
    }

    public static object Seq(Co co, Context cx){
        cx.Log("seq/" + co);
        object val = null;
        foreach(var k in co.nodes){
            val = cx.Step(k as Node);
            if(val == (object)false) return val;
            if(val == (object)@cont) return val;
        }
        return val;
    }

    public static object Act(Co co, Context cx){
        cx.Log("act/" + co);
        object val = null;
        foreach(var k in co.nodes){
            val = cx.Step(k as Node);
            if(val == (object)@cont) return val;
        }
        return val;
    }

}}
