/*
	author：@
	Last modified data:
	funtion todo:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Expression
{

    private string formula;

    private int index = 0;
    private char[] targetFormula = new char[100];
    public Expression(string formula)
    {
        this.formula = formula;
        for (int i = 0; i < formula.Length; i++)
        {
            targetFormula[i] = formula[i];
        }
    }

    public float GetReslut(int variableValue)
    {
        Stack<string> opSysbol = new Stack<string>();
        Stack<float> opNumber = new Stack<float>();

        opSysbol.Push("#");
        int len = formula.Length;
        bool is_minus = true;//判断是否为符号

        for (index = 0; index < len;)
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
            /*else if(targetFormula[index]=='^')
            {
                is_minus = false;
                opSysbol.Push("^");
                index++;
            }*/
            else if (targetFormula[index] == 'c')
            {
                is_minus = false;
                opSysbol.Push("cos");
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
                opSysbol.Push(targetFormula[index].ToString());
                index++;
            }
            //5.+-*/
            else
            {
                while (GetLevel(targetFormula[index]) <= GetLevel(opSysbol.Peek()[0]))
                {

                    float a2 = opNumber.Peek();
                    opNumber.Pop();
                    float a1 = opNumber.Peek();
                    opNumber.Pop();
                    string op = opSysbol.Peek();
                    opSysbol.Pop();
                    //Debug.Log(a2 + " " + a1 + " " + op);
                    float result = Operate(a1, op, a2);
                    opNumber.Push(result);
                }
                opSysbol.Push(targetFormula[index].ToString());
                index++;
            }
        }
        // Debug.Log(index);

        while (opSysbol.Peek() != "#")
        {
            float a2 = opNumber.Peek();
            opNumber.Pop();
            float a1 = opNumber.Peek();
            opNumber.Pop();
            string op = opSysbol.Peek();
            opSysbol.Pop();

            float res = Operate(a1, op, a2);
            opNumber.Push(res);
        }
        index = 0;
        return opNumber.Peek();
    }

    /* 返回运算符级别 */
    int GetLevel(char ch)
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
                return 2;
            case '(':
                return 0;
            case '#':
                return -1;
        };
        return 0;
    }

    float Translatrion()
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
        }
        return 0;
    }


}
