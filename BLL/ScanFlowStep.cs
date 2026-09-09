using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 扫描流程步骤业务层。
    /// </summary>
    public class ScanFlowStep
    {
        private readonly IScanFlowStep dal;

        public ScanFlowStep() { dal = XHS.DALFactory.ScanFlowStep.Create(); }

        public List<ScanFlowStepInfo> GetStepsByFlowId(int flowId)
        {
            return flowId <= 0 ? new List<ScanFlowStepInfo>() : dal.GetStepsByFlowId(flowId);
        }

        public List<ScanFlowStepInfo> GetStepsByFlowName(string flowName)
        {
            return string.IsNullOrWhiteSpace(flowName)
                ? new List<ScanFlowStepInfo>()
                : dal.GetStepsByFlowName(flowName.Trim());
        }

        public List<ScanFlowStepInfo> GetStepsByFlowCode(string flowCode)
        {
            return string.IsNullOrWhiteSpace(flowCode)
                ? new List<ScanFlowStepInfo>()
                : dal.GetStepsByFlowCode(flowCode.Trim());
        }

        /// <summary>
        /// 判断当前流程步骤是否已经完成。
        /// 非重复步骤一次扫描成功即完成；重复步骤比较页面传入的两个结束值。
        /// </summary>
        public bool IsStepCompleted(ScanFlowStepInfo step, string endValue, string compareValue)
        {
            return IsStepCompleted(step, endValue, compareValue, false);
        }

        public bool IsStepCompleted(ScanFlowStepInfo step, string endValue, string compareValue, bool manualEnd)
        {
            if (step == null)
            {
                return false;
            }

            if (manualEnd)
            {
                return true;
            }

            if (!step.AllowRepeat)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(step.EndControlId)
                || string.IsNullOrWhiteSpace(step.EndCompareControlId)
                || endValue == null
                || compareValue == null)
            {
                return false;
            }

            decimal endNumber;
            decimal compareNumber;
            if (decimal.TryParse(endValue.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out endNumber)
                && decimal.TryParse(compareValue.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out compareNumber))
            {
                return endNumber == compareNumber;
            }

            return string.Equals(endValue.Trim(), compareValue.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public string InsertSteps(List<ScanFlowStepInfo> infos)
        {
            string message = Validate(infos);
            if (!string.IsNullOrWhiteSpace(message)) return message;
            return Save(dal.InsertSteps(infos));
        }

        public string UpdateSteps(List<ScanFlowStepInfo> infos)
        {
            string message = Validate(infos);
            if (!string.IsNullOrWhiteSpace(message)) return message;
            return Save(dal.UpdateSteps(infos));
        }

        public string DeleteSteps(List<ScanFlowStepInfo> infos) { return Save(dal.DeleteSteps(infos)); }

        private static string Validate(List<ScanFlowStepInfo> infos)
        {
            if (infos == null) return "请提供流程步骤。";
            foreach (ScanFlowStepInfo info in infos)
            {
                if (info == null) continue;
                if (!info.FlowId.HasValue || info.FlowId.Value <= 0) return "FlowId 必须大于 0。";
                if (!info.StepNo.HasValue || info.StepNo.Value <= 0) return "步骤序号必须大于 0。";
                if (string.IsNullOrWhiteSpace(info.StepCode)) return "请填写步骤编码。";
                if (string.IsNullOrWhiteSpace(info.StepName)) return "请填写步骤名称。";
                if (string.IsNullOrWhiteSpace(info.ScanType)) return "请填写扫描类型。";
                if (string.IsNullOrWhiteSpace(info.RuleName)) return "请填写规则名称。";
                if (string.IsNullOrWhiteSpace(info.LabelType)) return "请填写标签类型。";
                bool hasEndControl = !string.IsNullOrWhiteSpace(info.EndControlId);
                bool hasCompareControl = !string.IsNullOrWhiteSpace(info.EndCompareControlId);
                if (hasEndControl != hasCompareControl) return "结束条件的两个控件 ID 必须同时填写。";
            }
            return string.Empty;
        }

        private static string Save(ParamterInfo parameterInfo)
        {
            IList source = new ArrayList(); source.Add(parameterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }
    }
}
