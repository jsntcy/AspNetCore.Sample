// **********************************************************************************************************
// This is an auto generated file and any changes directly applied to this file will be lost in next generation.
// Please DO NOT modify this file but instead, update ErrorDefinitions.xml and/or Error.tt.
// **********************************************************************************************************
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AspNetCore.Sample.Common.ErrorHandling
{
    public static class Errors
    {
        public static APIError InternalServerError()
        {
            return new APIError
            {
                ErrorCode = "internal_server_error",
                HttpStatusCode = 500,
                ErrorMessage = "An unexpected server error happened, please use operation id in response header to get help.",
                Retriable = false
            };
        }

        public static APIError Unauthorized()
        {
            return new APIError
            {
                ErrorCode = "unauthorized",
                HttpStatusCode = 401,
                ErrorMessage = "Bad credentials",
                Retriable = false
            };
        }

        public static APIError ETagNotMatch()
        {
            return new APIError
            {
                ErrorCode = "e_tag_not_match",
                HttpStatusCode = 412,
                ErrorMessage = "Your update to database conflicts with other operations, please retry.",
                Retriable = false
            };
        }

        private const string NotAvailable = "n/a";

        private static string GetString(object objValue)
        {
            if (objValue == null)
            {
                return "null";
            }

            return objValue.ToString();
        }

        private static string GetSnakeCaseFieldName<T>(Expression<Func<T, object>> objFieldExpr)
        {
            if (objFieldExpr == null)
            {
                return NotAvailable;
            }

            MemberExpression objMemberExpr = objFieldExpr.Body as MemberExpression;
            if (objMemberExpr == null)
            {
                var objUnaryExpr = objFieldExpr.Body as UnaryExpression;
                if (objUnaryExpr != null)
                {
                    objMemberExpr = objUnaryExpr.Operand as MemberExpression;
                }
            }

            if (objMemberExpr == null)
            {
                throw new NotSupportedException();
            }

            var strFieldName = (objMemberExpr.Member as PropertyInfo).Name;

            return ToSnakeCaseName(strFieldName);
        }

        private static string ToSnakeCaseName(string strPropertyName)
        {
            if (string.IsNullOrEmpty(strPropertyName))
            {
                return strPropertyName;
            }

            var objSnakeCaseName = new StringBuilder();

            var objUpperPart = new StringBuilder();

            for (int index = 0; index < strPropertyName.Length; index++)
            {
                if (char.IsUpper(strPropertyName[index]))
                {
                    objUpperPart.Append(char.ToLower(strPropertyName[index], CultureInfo.CurrentCulture));

                    if (index == strPropertyName.Length - 1)
                    {
                        if (objUpperPart.Length != strPropertyName.Length)
                        {
                            objSnakeCaseName.Append("_");
                        }

                        objSnakeCaseName.Append(objUpperPart);

                        objUpperPart.Clear();
                    }
                }
                else
                {
                    if (objUpperPart.Length > 0)
                    {
                        if (objUpperPart.Length != index)
                        {
                            objSnakeCaseName.Append("_");
                        }

                        if (objUpperPart.Length > 1)
                        {
                            objSnakeCaseName.Append(objUpperPart.ToString().Substring(0, objUpperPart.Length - 1));
                            objSnakeCaseName.Append("_");
                            objSnakeCaseName.Append(objUpperPart.ToString().Substring(objUpperPart.Length - 1));
                        }
                        else
                        {
                            objSnakeCaseName.Append(objUpperPart);
                        }

                        objUpperPart.Clear();
                    }

                    objSnakeCaseName.Append(strPropertyName[index]);
                }
            }

            return objSnakeCaseName.ToString();
        }
    }
}
