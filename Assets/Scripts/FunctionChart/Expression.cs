/*
 * UGUIChartLib
 * Copyright © 2019 w199753. 
 * feedback:http://15384855139@163.com
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChartLib
{

    public class Expression
    {
        private const int initSize = 100;

        private int index = -1;
        private int realLength = -1;

        private char[] targetFormula;
        private Stack<string> opSysbol;
        private Stack<float> opNumber;

        public Expression()
        {
            targetFormula = new char[initSize];
            opSysbol = new Stack<string>(4*initSize);
            opNumber = new Stack<float>(4*initSize);
        }

        public void SetFormula(string formula)
        {
            if (string.IsNullOrEmpty(formula))
            {
                throw new System.Exception("the formulaStr is null or empty,check it");
            }

            ResetValue();

            int j = 0;
            int len = formula.Length;
            for (int i = 0; i < len; i++)
            {
                if (formula[i] != ' ')
                    targetFormula[j++] = formula[i];
            }
            realLength = j;
        }

        //清空容器
        private void ResetValue()
        {
            for (int i = 0; i < initSize; i++)
            {
                targetFormula[i] = ' ';
            }
            opSysbol.Clear();
            opNumber.Clear();
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="variableValue">自变量需替换的值</param>
        /// <returns></returns>
        public float GetReslut(float variableValue)
        {
            if (realLength < 0)
            {
                Debug.LogError("please set formula first!");
                return 0;
            }

            opSysbol.Push("#");
            bool is_minus = true;//判断是否为符号

            for (index = 0; index < realLength;)
            {
                if (targetFormula[index] == '-' && is_minus)
                {
                    opNumber.Push(0);
                    opSysbol.Push("-");
                    index++;
                }
                else if (targetFormula[index] == ')')
                {
                    is_minus = false;
                    index++;

                    while (opSysbol.Peek() != "(")
                    {
                        if (opNumber.Count < 2)
                        {
                            return 0;
                        }
                        float a2 = opNumber.Peek();
                        opNumber.Pop();
                        float a1 = opNumber.Peek();
                        opNumber.Pop();
                        string op = opSysbol.Peek();
                        opSysbol.Pop();

                        float res = Operate(a1, op, a2);
                        opNumber.Push(res);
                    }
                    opSysbol.Pop();
                }
                else if (targetFormula[index] == 'c' && targetFormula[index + 1] == 'o' && targetFormula[index + 2] == 's')
                {
                    is_minus = false;
                    opSysbol.Push("cos");
                    opNumber.Push(1);
                    index += 3;
                }
                else if (targetFormula[index] == 's' && targetFormula[index + 1] == 'i' && targetFormula[index + 2] == 'n')
                {
                    is_minus = false;
                    opSysbol.Push("sin");
                    opNumber.Push(1);
                    index += 3;
                }
                else if (targetFormula[index] == 't' && targetFormula[index + 1] == 'a' && targetFormula[index + 2] == 'n')
                {
                    is_minus = false;
                    opSysbol.Push("tan");
                    opNumber.Push(1);
                    index += 3;
                }
                //lg以10为底
                else if (targetFormula[index] == 'l' && targetFormula[index + 1] == 'g')
                {
                    is_minus = false;
                    opSysbol.Push("lg");
                    opNumber.Push(1);
                    index += 2;
                }
                //ln以e为底
                else if (targetFormula[index] == 'l' && targetFormula[index + 1] == 'n')
                {
                    is_minus = false;
                    opSysbol.Push("ln");
                    opNumber.Push(1);
                    index += 2;
                }
                //找到变量
                else if (targetFormula[index] == 'x')
                {
                    is_minus = false;
                    opNumber.Push(variableValue);
                    index++;
                }
                //3.数字
                else if (targetFormula[index] >= '0' && targetFormula[index] <= '9')
                {
                    is_minus = false;
                    opNumber.Push(Translatrion());
                }
                //4.左括号
                else if (targetFormula[index] == '(')
                {
                    is_minus = true;
                    opSysbol.Push("(");
                    index++;
                }
                //5.+-*/^
                else
                {
                    while (GetLevel(targetFormula[index]) <= GetLevel(opSysbol.Peek()[0]))
                    {
                        if (opNumber.Count < 2)
                        {
                            return 0;
                        }
                        float a2 = opNumber.Peek();
                        opNumber.Pop();
                        float a1 = opNumber.Peek();
                        opNumber.Pop();
                        string op = opSysbol.Peek();
                        opSysbol.Pop();
                        float result = Operate(a1, op, a2);
                        opNumber.Push(result);
                    }
                    if (targetFormula[index] == '+')
                        opSysbol.Push("+");
                    else if (targetFormula[index] == '-')
                        opSysbol.Push("-");
                    else if (targetFormula[index] == '*')
                        opSysbol.Push("*");
                    else if (targetFormula[index] == '/')
                        opSysbol.Push("/");
                    else if (targetFormula[index] == '^')
                        opSysbol.Push("^");
                    index++;
                }
            }
            // Debug.Log(index);

            while (opSysbol.Peek() != "#")
            {
                if (opNumber.Count < 2)
                {
                    return 0;
                }
                float a2 = opNumber.Peek();
                opNumber.Pop();
                float a1 = opNumber.Peek();
                opNumber.Pop();
                string op = opSysbol.Peek();
                opSysbol.Pop();

                float res = Operate(a1, op, a2);
                opNumber.Push(res);
            }

            return opNumber.Peek();
        }

        /* 返回运算符级别 */
        private int GetLevel(char ch)
        {
            switch (ch)
            {
                case '+':
                case '-':
                    return 1;
                case '*':
                case '/':
                case 'c':
                case '^':
                case 'l':
                case 's':
                case 't':
                    return 2;
                case '(':
                    return 0;
                case '#':
                    return -1;
            };
            return 0;
        }

        private float Translatrion()
        {
            float integer = 0.0f;    // 整数部分
            float remainder = 0.0f;  // 余数部分

            int pos = index;
            //Debug.Log(pos);
            while (targetFormula[pos] >= '0' && targetFormula[pos] <= '9')
            {
                integer *= 10;
                integer += (targetFormula[pos] - '0');
                pos++;

            }

            if (targetFormula[pos] == '.')
            {
                pos++;
                int c = 1;
                while (targetFormula[pos] >= '0' && targetFormula[pos] <= '9')
                {
                    float t = targetFormula[pos] - '0';
                    t *= Mathf.Pow(0.1f, c);
                    c++;
                    remainder += t;
                    pos++;
                }
            }
            index = pos++;
            return integer + remainder;
        }


        float Operate(float a1, string op, float a2)
        {
            switch (op)
            {
                case "+":
                    return a1 + a2;
                case "-":
                    return a1 - a2;
                case "*":
                    return a1 * a2;
                case "/":
                    return a1 / a2;
                case "lg":
                    return a1 * Mathf.Log10(a2);
                case "ln":
                    return a1 * Mathf.Log(a2);
                case "^":
                    return Mathf.Pow(a1, a2);
                case "cos":
                    return Mathf.Cos(a1 * a2);
                case "sin":
                    return Mathf.Sin(a1 * a2);
                case "tan":
                    return Mathf.Tan(a1 * a2);
            }
            return 0;
        }

    }

}