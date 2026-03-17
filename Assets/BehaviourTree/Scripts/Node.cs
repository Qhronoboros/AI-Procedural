using System.Collections.Generic;

public class Node
{
    public enum Status
    {
        Success,
        Failure,
        Running
    }

    public string name;
    public List<Node> children = new();
    protected int currentChild;

    public Node(string name = "Node")
    {
        this.name = name;
    }
    
    public void AddChild(Node child) => children.Add(child);
    
    public virtual Status Process() => children[currentChild].Process();
    
    public virtual void Reset()
    {
        currentChild = 0;
        foreach (Node child in children)
        {
            child.Reset();
        }
    }
}
