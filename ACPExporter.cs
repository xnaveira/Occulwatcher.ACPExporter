using OccultWatcher.SDK;
using Occulwatcher.ACPExporter.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Occulwatcher.ACPExporter
{
    public class ACPExporterConfig
    {
        public string ConfigFilePath { set; get; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ACPExport.txt";
        public string ConfigHeader { set; get; } = "#Interval 60\r\n#Filter No Filter\r\n#Binning 2\r\n";

    }
    public class ACPExporter : IOWAddin
    {
        private IOWHost m_HostInfo = null;
        private IOWResourceProvider m_ResourceProvider = null;
        public OWAddinAction[] ADDIN_ACTIONS = null;
        private ACPExporterConfig config = new ACPExporterConfig();

        internal string GetResourceString(string resourceName, string defaultValue)
        {
            if (m_ResourceProvider == null)
                // Backwards compatibility with older versions of OW
                return defaultValue;
            else
                return m_ResourceProvider.GetResourceString(resourceName, defaultValue);
        }
        public void Configure(System.Windows.Forms.IWin32Window owner)
        {
            configDialog cd = new configDialog(config);
            cd.ShowDialog(owner);
        }

        private string GetStarDirection(IOWUserConfig userSettings, double azimuth)
        {
            if (userSettings != null &&
                userSettings.AzimuthsInDegrees)
                return string.Format("@{0}°", (int)azimuth);

            if (azimuth <= 0 + 22.5 || azimuth > 360 - 22.5)
                return GetResourceString("direction.N", "N");
            if (azimuth <= 45 * 1 + 22.5 && azimuth > 45 * 1 - 22.5)
                return GetResourceString("direction.NE", "NE");
            if (azimuth <= 45 * 2 + 22.5 && azimuth > 45 * 2 - 22.5)
                return GetResourceString("direction.E", "E");
            if (azimuth <= 45 * 3 + 22.5 && azimuth > 45 * 3 - 22.5)
                return GetResourceString("direction.SE", "SE");
            if (azimuth <= 45 * 4 + 22.5 && azimuth > 45 * 4 - 22.5)
                return GetResourceString("direction.S", "S");
            if (azimuth <= 45 * 5 + 22.5 && azimuth > 45 * 5 - 22.5)
                return GetResourceString("direction.SW", "SW");
            if (azimuth <= 45 * 6 + 22.5 && azimuth > 45 * 6 - 22.5)
                return GetResourceString("direction.W", "W");
            if (azimuth <= 45 * 7 + 22.5 && azimuth > 45 * 7 - 22.5)
                return GetResourceString("direction.NW", "NW");

            return string.Empty;
        }

        public void ExecuteAction(int actionId, IOWAsteroidEvent astEvent, System.Windows.Forms.IWin32Window owner, OWEventArguments eventArgs)
        {
            if (actionId == 1)
            {
                if (astEvent != null)
                {
                    IOWLocalEventData siteData = astEvent as IOWLocalEventData;
                    IOWUserConfig userSettings = astEvent as IOWUserConfig;
                    IOWAsteroidEvent2 evt2 = astEvent as IOWAsteroidEvent2;

                    //TODO: Use the configured format string

                    string errorInTime = string.Empty;
                    if (evt2 != null &&
                        evt2.ErrorInTime != -1)
                    {
                        if (evt2.ErrorInTime > 90)
                        {
                            double err = evt2.ErrorInTime / 60;
                            errorInTime = string.Format(" +/- {0} min", err.ToString("#0.0"));
                        }
                        else
                            errorInTime = string.Format(" +/- {0} sec", evt2.ErrorInTime.ToString("#0"));
                    }

                    //string maxDuration = string.Empty;
                    //if (astEvent.MaxDuration > 90)
                    //{
                    //    double dur = astEvent.MaxDuration / 60;
                    //    maxDuration = string.Format("{0} min", dur.ToString("#0.0"));
                    //}
                    //else
                    //    maxDuration = string.Format("{0} sec", astEvent.MaxDuration.ToString("#0"));

                    //string eventInfo =
                    //    astEvent.AsteroidName + " occults " + astEvent.StarName + "; m = " + astEvent.StarMagnitude.ToString() + "; " + ((int)Math.Round(siteData.StarAltitude)).ToString() + "° " + GetStarDirection(userSettings, siteData.StarAzimuth) + "\r\n" +
                    //    siteData.EventTime.ToString("dd MMM; HH:mm:ss UT") + errorInTime + "; dur: " + maxDuration + "; drop: " + astEvent.MagnitudeDrop.ToString("#0.0") + "m;";

                    string eventInfo =
                        "#WaitUntil 1," + 
                        siteData.EventTime.AddSeconds(Convert.ToInt32(astEvent.MaxDuration/2)).ToString("dd/MM/yyyy HH:mm:ss").Replace("-","/") + 
                        "\r\n" + 
                        astEvent.AsteroidName + 
                        "\t" + 
                        evt2.Occelmnt.StarRAHours.ToString().Replace(",",".") + 
                        "\t" + 
                        evt2.Occelmnt.StarDEDeg.ToString().Replace(",", ".") + 
                        "\r\n";

                    Clipboard.SetText(eventInfo);
                    bool existed = false;
                    if (File.Exists(config.ConfigFilePath))
                    {
                        existed = true;
                    }
                        using (StreamWriter writer = new StreamWriter(config.ConfigFilePath, true))
                    {
                        if (!existed)
                        {
                            writer.Write(config.ConfigHeader);
                        }
                        writer.Write(eventInfo);
                    }

                    MessageBox.Show(
                        owner,
                        GetResourceString("OWAE.doneText", "The following info has been copied to the ACP file") + 
                            "\r\n\r\n" + 
                            eventInfo + 
                            "\r\n" + 
                            config.ConfigFilePath,
                        GetResourceString("OWAE.AddinName", "ACP Exporter"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }

            //if (actionId == 2)
            //{
            //    OWEventArguments args = eventArgs;
            //}

            if (actionId == 7)
            {
                if (astEvent != null)
                {
                    IOWLocalEventData siteData = astEvent as IOWLocalEventData;
                    IOWUserConfig userSettings = astEvent as IOWUserConfig;
                    IOWAsteroidEvent2 evt2 = astEvent as IOWAsteroidEvent2;
                    IOWStationInfo stationInfo = astEvent as IOWStationInfo;

                    //TODO: Observation Report will be prepared here
                }
            }
        }

        public void FinalizeAddin()
        {
            // Nothing
        }

        public OWAddinInfo GetAddinInfo()
        {
            return new OWAddinInfo(GetResourceString("OWAE.AddinName", "ACP Exporter"), true, Properties.Resources.ACPExporterIcon.ToBitmap());
        }

        public void InitializeAddin(IOWHost hostInfo)
        {
            m_HostInfo = hostInfo;
            m_ResourceProvider = hostInfo as IOWResourceProvider;

            ADDIN_ACTIONS = new OWAddinAction[]
            {
                new OWAddinAction(1, GetResourceString("OWAE.Action", "Export event to ACP"), OWAddinActionType.SelectedEventAction, Properties.Resources.ACPExporterIcon.ToBitmap()),
                new OWAddinAction(2, GetResourceString("OWAE.Action", "Export event to ACP"), OWAddinActionType.EventReceiver, Properties.Resources.ACPExporterIcon.ToBitmap()),
                new OWAddinAction(7, string.Empty, OWAddinActionType.ReportProcessor, null, Color.White)
            };

        }

        public bool ShouldDisplayAction(int actionId, IOWAsteroidEvent astEvent)
        {
            // Our actions will be always displayed for all events
            return true;
        }

        public IEnumerable<OWAddinAction> SupportedActions()
        {
            return ADDIN_ACTIONS;
        }
    }
}

