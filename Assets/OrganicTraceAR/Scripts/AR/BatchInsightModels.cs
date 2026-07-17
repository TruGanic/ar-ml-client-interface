using System;

namespace OrganicTraceAR.AR
{
    [Serializable]
    public class BatchInsightResponse
    {
        public string batchId;
        public string produceType;
        public string status;
        public SummaryData summary;
        public TransportData transport;
        public string[] explanations;
        public DataQualityData dataQuality;
        public ProofData proof;
    }

    [Serializable]
    public class SummaryData
    {
        public string organicGrade;
        public int organicScore;
        public int freshnessDaysSinceHarvest;
        public int coldChainComplianceScore;
        public int overallTrustScore;
    }

    [Serializable]
    public class TransportData
    {
        public MinMaxAvgData tempC;
        public MinMaxAvgData humidityPct;
        public float durationHours;
        public string[] flags;
    }

    [Serializable]
    public class MinMaxAvgData
    {
        public float min;
        public float max;
        public float avg;
    }

    [Serializable]
    public class DataQualityData
    {
        public string[] missingFields;
        public string[] anomalies;
    }

    [Serializable]
    public class ProofData
    {
        public string txId;
        public string merkleRoot;
        public string invoiceHash;
    }
}
