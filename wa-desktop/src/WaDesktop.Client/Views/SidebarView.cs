using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Client.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace WaDesktop.Client.Views
{
    public partial class SidebarView : UserControl, ISidebarView
    {
        private readonly ContextMenuStrip _phoneContextMenu;
        private readonly Timer _refreshTimer;

        public SidebarView()
        {
            InitializeComponent();


            components = components ?? new System.ComponentModel.Container();
            _refreshTimer = new Timer(components);
            _refreshTimer.Interval = 60000; // 60 detik
            _refreshTimer.Tick += (s, e) => RefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode)
            {
                _refreshTimer.Start();
            }
        }

        // ── ISidebarView ──

        public bool IsLoading
        {
            set { this.InvokeIfRequired(() => { }); }
        }

        public event EventHandler<PhoneNumberSelectedEventArgs> PhoneNumberSelected;
        public event EventHandler RefreshRequested;

        public void LoadPhoneNumbers(IList<PhoneNumberNode> nodes)
        {
            this.InvokeIfRequired(() =>
            {
            });
        }

        public void UpdateUsageSummary(WaWabaUsageSummary summary)
        {
            this.InvokeIfRequired(() =>
            {
                if (summary == null) return;
                var idId = new System.Globalization.CultureInfo("id-ID");

                tbMarketingCount.Text = summary.MarketingCost.ToString("C2", idId);
                tbUtilityCount.Text = summary.UtilityCost.ToString("C2", idId);
                tbAuthenticationCount.Text = summary.AuthCost.ToString("C2", idId);
                tbServiceCount.Text = summary.ServiceCost.ToString("C2", idId);

                tbBillMeta.Text = summary.TotalCost.ToString("C2", idId);
            });
        }

        public void UpdateWebhookStatus(bool isRunning, string message)
        {
            this.InvokeIfRequired(() =>
            {
                treeView.ShowNodeToolTips = true; // Enable tooltips
                
                var nodeKey = "webhook_status";
                TreeNode[] nodes = treeView.Nodes.Find(nodeKey, false);
                TreeNode node;
                if (nodes.Length > 0)
                {
                    node = nodes[0];
                }
                else
                {
                    node = new TreeNode { Name = nodeKey };
                    treeView.Nodes.Insert(0, node); // Letakkan di paling atas
                }

                node.Text = isRunning ? "Webhook: Running" : "Webhook: Stopped / Error";
                node.ToolTipText = message;
                
                // Gunakan "Play1Normal_.png" (index 3) dan "Stop1Disabled_.png" (index 0)
                node.ImageKey = isRunning ? "Play1Normal_.png" : "Stop1Disabled_.png";
                node.SelectedImageKey = node.ImageKey;
            });
        }

        private static TreeNode BuildTreeNode(PhoneNumberNode node)
        {
            var isGroup = string.IsNullOrEmpty(node.PhoneNumberId);
            var tn = new TreeNode
            {
                Text = isGroup
                    ? node.DisplayName
                    : !string.IsNullOrEmpty(node.DisplayName)
                        ? $"{node.DisplayName} ({node.DisplayPhoneNumber})"
                        : node.DisplayPhoneNumber,
                Tag = node
            };
            foreach (var child in node.Children)
                tn.Nodes.Add(BuildTreeNode(child));
            return tn;
        }

        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.Node == null) return;
    
        }

        private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node?.Tag is PhoneNumberNode node && !string.IsNullOrEmpty(node.PhoneNumberId))
            {
                PhoneNumberSelected?.Invoke(this,
                    new PhoneNumberSelectedEventArgs(node.PhoneNumberId, node.DisplayPhoneNumber, node.DisplayName));
            }
        }
    }
}
