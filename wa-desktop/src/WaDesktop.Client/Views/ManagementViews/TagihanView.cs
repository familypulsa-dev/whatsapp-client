using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WaDesktop.Domain.Entities;
using WaDesktop.Client.Extensions;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Views.ManagementViews
{
    public partial class TagihanView : UserControl, IViewBase
    {
        public TagihanView()
        {
            InitializeComponent();
            dtpStart.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-3);
            dtpEnd.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        public IList<WaWabaUsageSummary> DataSource
        {
            set
            {
                this.InvokeIfRequired(() =>
                {
                    dataGridView.Rows.Clear();
                    foreach (var s in value)
                    {
                        dataGridView.Rows.Add(
                            s.MonthPeriod,
                            s.MarketingCost,
                            s.UtilityCost,
                            s.AuthCost,
                            s.ServiceCost,
                            s.TotalVolume,
                            s.TotalCost
                        );
                    }
                });
            }
        }

        public DateTime FilterStart => dtpStart.Value;
        public DateTime FilterEnd => dtpEnd.Value.AddMonths(1).AddDays(-1);

        public string SelectedWabaId
        {
            get
            {
                try { return cmbWaba.SelectedValue?.ToString() ?? ""; }
                catch { return ""; }
            }
        }

        public void SetWabaDataSource(IList<Waba> wabas)
        {
            this.InvokeIfRequired(() =>
            {
                cmbWaba.SelectedIndexChanged -= cmbWaba_SelectedIndexChanged;

                var list = new List<Waba>
                {
                    new Waba { WabaId = "", Name = "-- Semua WABA --" }
                };
                list.AddRange(wabas);

                cmbWaba.DataSource = list;
                cmbWaba.DisplayMember = "Name";
                cmbWaba.ValueMember = "WabaId";

                if (list.Count > 0) cmbWaba.SelectedIndex = 0;

                cmbWaba.SelectedIndexChanged += cmbWaba_SelectedIndexChanged;
            });
        }

        public bool IsLoading { set => this.InvokeIfRequired(() => { Cursor = value ? Cursors.WaitCursor : Cursors.Default; }); }

        public event EventHandler FilterClicked;
        public event EventHandler<string> WabaFilterChanged;

        private void btnFilter_Click(object sender, EventArgs e)
        {
            FilterClicked?.Invoke(this, EventArgs.Empty);
        }

        private void cmbWaba_SelectedIndexChanged(object sender, EventArgs e)
        {
            WabaFilterChanged?.Invoke(this, cmbWaba.SelectedValue?.ToString());
        }
    }
}
