namespace TheOne.Avalonia.Workflow;

public sealed partial class WorkflowDocument
{
    private Dictionary<NodeId, WorkflowNode>? _nodeIndex;
    private GraphIndex? _connectionIndex;

    internal bool ContainsNode(WorkflowNode node) => _nodes.ContainsKey(node);
    internal bool ContainsConnection(WorkflowConnection connection) => _connections.ContainsKey(connection);

    internal WorkflowNode? FindNode(string? id)
    {
        if (_nodeIndex == null)
        {
            _nodeIndex = [];
            // Invalid duplicate IDs still resolve to the first collection item, as before.
            foreach (var node in Nodes) _nodeIndex.TryAdd(new NodeId(node.Id), node);
        }

        return _nodeIndex.GetValueOrDefault(new NodeId(id));
    }

    private GraphIndex ConnectionIndex => _connectionIndex ??= new GraphIndex(Connections);

    // Wrapping the ID preserves the distinction between null and empty draft IDs.
    private readonly record struct NodeId(string? Value);

    private sealed class GraphIndex
    {
        public readonly Dictionary<NodeId, List<NodeId>> Outgoing = [], Incoming = [];
        public readonly Dictionary<(string Node, string Port), int> OutputCounts = [], InputCounts = [];
        public readonly Dictionary<(string Source, string Output, string Target, string Input), int> Edges = [];
        private Dictionary<NodeId, int>? _components;

        public GraphIndex(IEnumerable<WorkflowConnection> connections)
        {
            foreach (var edge in connections)
            {
                Add(Outgoing, new NodeId(edge.SourceNodeId), new NodeId(edge.TargetNodeId));
                Add(Incoming, new NodeId(edge.TargetNodeId), new NodeId(edge.SourceNodeId));
                var output = (edge.SourceNodeId, edge.SourcePortId);
                var input = (edge.TargetNodeId, edge.TargetPortId);
                var endpoints = (edge.SourceNodeId, edge.SourcePortId, edge.TargetNodeId, edge.TargetPortId);
                OutputCounts[output] = OutputCounts.GetValueOrDefault(output) + 1;
                InputCounts[input] = InputCounts.GetValueOrDefault(input) + 1;
                Edges[endpoints] = Edges.GetValueOrDefault(endpoints) + 1;
            }
        }

        private static void Add(Dictionary<NodeId, List<NodeId>> graph, NodeId from, NodeId to)
        {
            if (!graph.TryGetValue(from, out var next)) graph.Add(from, next = []);
            next.Add(to);
        }

        public HashSet<NodeId> Upstream(NodeId target)
        {
            HashSet<NodeId> seen = [];
            var pending = new Stack<NodeId>();
            pending.Push(target);
            while (pending.TryPop(out var id))
            {
                if (!seen.Add(id)) continue;
                if (Incoming.TryGetValue(id, out var sources))
                    foreach (var source in sources) pending.Push(source);
            }

            return seen;
        }

        public Dictionary<NodeId, int> Components()
        {
            if (_components != null) return _components;
            HashSet<NodeId> visited = [];
            List<NodeId> order = [];
            var pending = new Stack<(NodeId Id, bool Finished)>();
            foreach (var start in Outgoing.Keys.Concat(Incoming.Keys))
            {
                if (visited.Contains(start)) continue;
                pending.Push((start, false));
                while (pending.TryPop(out var entry))
                {
                    if (entry.Finished)
                    {
                        order.Add(entry.Id);
                        continue;
                    }

                    if (!visited.Add(entry.Id)) continue;
                    pending.Push((entry.Id, true));
                    if (Outgoing.TryGetValue(entry.Id, out var targets))
                        foreach (var target in targets)
                            if (!visited.Contains(target)) pending.Push((target, false));
                }
            }

            var components = new Dictionary<NodeId, int>();
            var stack = new Stack<NodeId>();
            for (var i = order.Count - 1; i >= 0; i--)
            {
                if (components.ContainsKey(order[i])) continue;
                var component = components.Count;
                stack.Push(order[i]);
                while (stack.TryPop(out var id))
                {
                    if (!components.TryAdd(id, component)) continue;
                    if (Incoming.TryGetValue(id, out var sources))
                        foreach (var source in sources) stack.Push(source);
                }
            }

            return _components = components;
        }
    }
}
