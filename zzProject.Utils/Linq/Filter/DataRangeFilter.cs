using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using LinqKit;
using System.Data.Objects;

//See EntityFunctions: http://msdn.microsoft.com/en-us/library/dd383061.aspx
//See Canonical Functions: http://msdn.microsoft.com/en-us/library/bb738626.aspx
//See: http://msdn.microsoft.com/en-us/library/cc716715.aspx
//See: http://msdn.microsoft.com/en-us/library/dd490950.aspx

namespace zzProject.Utils.Linq.Filter
{
    public class DateRangeFilter
    {
        private DateTime _start;
        private DateTime _end;
        private TwoParamsFindOption _twoParamsFindOption;
        private OneParamFindOption _oneParamFindOption;
        private ParamsType _paramsType;
        private bool _dismissTime;

        public enum TwoParamsFindOption
        {
            wide, //Includes any event where some time of the event is between the two dates
            startInside, //Includes any event where the begin date is betwen the two dates
            endInside, //Includes any event where the end date is between the two dates
            narrow  //Includes any event that begins and ends between the two dates
        }

        public enum OneParamFindOption
        {
            startWide, //Includes any event where some time of the event is after the start or the start is after the date
            startNarrow, //Includes any event where the start is after the date
            endWide, //Includes any event where some time of the event is before the end or the end is before the date
            endNarrow  //Includes any event where the end is before the date
        }

        private enum ParamsType
        {
            twoParams,
            oneParam
        }

        public DateRangeFilter(DateTime start, DateTime end, TwoParamsFindOption twoParamsFindOption, bool dismissTime)
        {
            this._start = start;
            this._end = end;
            this._twoParamsFindOption = twoParamsFindOption;
            this._paramsType = ParamsType.twoParams;
        }

        public DateRangeFilter(DateTime date, OneParamFindOption oneParamFindOption, bool dismissTime)
        {
            this._oneParamFindOption = oneParamFindOption;
            switch (this._oneParamFindOption)
            {
                case OneParamFindOption.startWide:
                    this._start = date;
                    break;
                case OneParamFindOption.startNarrow:
                    this._start = date;
                    break;
                case OneParamFindOption.endWide:
                    this._end = date;
                    break;
                case OneParamFindOption.endNarrow:
                    this._end = date;
                    break;
            }
            this._paramsType = ParamsType.oneParam;
        }

        public IQueryable<T> Filter<T>(IQueryable<T> query, Expression<Func<T, DateTime?>> startProperty, Expression<Func<T, DateTime?>> endProperty)
        {
            //result = query.AsExpandable().Where(c => startProperty.Invoke(c).Value < this._start);

            IQueryable<T> result;
            switch (this._paramsType)
            {
                case ParamsType.oneParam:
                    switch (this._oneParamFindOption)
                    {
                        case OneParamFindOption.startWide:
                            {
                                if (this._dismissTime)
                                {
                                    result = query.AsExpandable().Where(c =>
                                        (EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._start) && EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) >= EntityFunctions.TruncateTime(this._start))
                                        || EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._start));
                                }
                                else
                                {
                                    result = query.AsExpandable().Where(c =>
                                        (startProperty.Invoke(c).Value <= this._start && startProperty.Invoke(c).Value >= this._start)
                                        || startProperty.Invoke(c).Value <= this._start);
                                }
                            }
                            break;
                        case OneParamFindOption.startNarrow:
                            {
                                if (this._dismissTime)
                                {
                                    result = query.AsExpandable().Where(c => EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._start));
                                }
                                else
                                {
                                    result = query.AsExpandable().Where(c => startProperty.Invoke(c).Value <= this._start);
                                }
                            }
                            break;
                        case OneParamFindOption.endWide:
                            {
                                if (this._dismissTime)
                                {
                                    result = query.AsExpandable().Where(c =>
                                        (EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._end) && EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) >= EntityFunctions.TruncateTime(this._end))
                                        || EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._end));
                                }
                                else
                                {
                                    result = query.AsExpandable().Where(c =>
                                        (endProperty.Invoke(c).Value <= this._end && endProperty.Invoke(c).Value >= this._end)
                                        || endProperty.Invoke(c).Value <= this._end);
                                }
                            }
                            break;
                        case OneParamFindOption.endNarrow:
                            {
                                if (this._dismissTime)
                                {
                                    result = query.AsExpandable().Where(c => EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._end));
                                }
                                else
                                {
                                    result = query.AsExpandable().Where(c => endProperty.Invoke(c).Value <= this._end);
                                }
                            }
                            break;
                        default:
                            result = query;
                            break;
                    }
                    break;
                case ParamsType.twoParams:
                    switch (this._twoParamsFindOption)
                    {
                        case TwoParamsFindOption.wide:
                            {
                                if (this._dismissTime)
                                {
                                    result = query.AsExpandable().Where(c =>
                                      (EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) >= EntityFunctions.TruncateTime(this._start) && EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._end)) ||
                                      (EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) >= EntityFunctions.TruncateTime(this._start) && EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._end)) ||
                                      (EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) >= EntityFunctions.TruncateTime(this._start) && EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._end)));
                                }
                                else
                                {
                                    result = query.AsExpandable().Where(c =>
                                          (startProperty.Invoke(c).Value >= this._start && startProperty.Invoke(c).Value <= this._end) ||
                                          (endProperty.Invoke(c).Value >= this._start && endProperty.Invoke(c).Value <= this._end) ||
                                          (startProperty.Invoke(c).Value >= this._start && endProperty.Invoke(c).Value <= this._end));
                                }
                            }
                            break;
                        case TwoParamsFindOption.startInside:
                            {
                                if (this._dismissTime)
                                {
                                    result = query.AsExpandable().Where(c => EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) >= EntityFunctions.TruncateTime(this._start) && EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._end));
                                }
                                else
                                {
                                    result = query.AsExpandable().Where(c => startProperty.Invoke(c).Value >= this._start && startProperty.Invoke(c).Value <= this._end);
                                }
                            }
                            break;

                        case TwoParamsFindOption.endInside:
                            {
                                if (this._dismissTime)
                                {
                                    result = query.AsExpandable().Where(c => EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) >= EntityFunctions.TruncateTime(this._start) && EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._end));
                                }
                                else
                                {
                                    result = query.AsExpandable().Where(c => endProperty.Invoke(c).Value >= this._start && endProperty.Invoke(c).Value <= this._end);
                                }
                            }
                            break;
                        case TwoParamsFindOption.narrow:
                            {
                                if (this._dismissTime)
                                {
                                    result = query.AsExpandable().Where(c => EntityFunctions.TruncateTime(startProperty.Invoke(c).Value) <= EntityFunctions.TruncateTime(this._start) && EntityFunctions.TruncateTime(endProperty.Invoke(c).Value) >= EntityFunctions.TruncateTime(this._end));
                                }
                                else
                                {
                                    result = query.AsExpandable().Where(c => startProperty.Invoke(c).Value <= this._start && endProperty.Invoke(c).Value >= this._end);
                                }
                            }
                            break;
                        default:
                            result = query;
                            break;
                    }
                    break;
                default:
                    result = query;
                    break;
            }
            return result;
        }
    }
}
