using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalizadorSemantico
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            button1.Enabled = false;
            this.label3.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            label3.Text = "";
            this.AnalisisLexico();

            int contador_errores = 0;
            for (int x = 0; x < dataGridView1.RowCount; x++)
            {
                if ((dataGridView1.Rows[x].Cells[1].Value.ToString()).Equals("ERROR"))
                {
                    contador_errores += 1;
                    dataGridView1.Rows[x].DefaultCellStyle.BackColor = Color.Red;

                }
            }
            if (contador_errores > 0)
            {
                button1.Enabled = false;

                this.label3.Text = "Errores Lexicos =" + contador_errores;
                this.label3.Visible = true;
            }
            else
            {
                this.label3.Text = "Ningun error Lexico encontrado";
                this.label3.Visible = true;
                button1.Enabled = true;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < pasosPasarAC; x++)
            {

                pasarAC[x] = "";


            }
            pasosPasarAC = 0;
            dataGridView2.Rows.Clear();
            for (int i = 0; i < variables.Length; i++)
            {
                variables[i] = "0";
            }
            inicioCorrec = "0";
            procesoCorrec = "0";
            existProcs = "0";
            existFin = "0";
            contErrorCompilar = 0;
            contVariables = 0;
            this.AnalisisSintactico();


            for (int x = 0; x < dataGridView2.RowCount; x++)
            {
                if ((dataGridView2.Rows[x].Cells[0].Value.ToString()).Equals("Error"))
                {

                    dataGridView2.Rows[x].DefaultCellStyle.BackColor = Color.Red;
                    contErrorCompilar += 1;
                }
            }

        }

        string unir = "";
        string espacio = "[' ']";
        string saltoLinea = "['\n']";

        string unirString = "";
        string unirCom = "";

        int contColumnas = 1;
        int contLineas = 1;

        public void AnalisisLexico()
        {
            dataGridView1.Rows.Clear();

            contColumnas = 1;
            contLineas = 1;


            char validarCom = '0';
            char validarCadString = '0';

            string coment = "@";
            string cadString = "[\"]";
            string text = textBox1.Text;



            foreach (char letra in text)
            {
                string letra2 = letra.ToString();


                if (Regex.IsMatch(letra2, cadString))
                {
                    if (validarCadString.Equals('0'))
                    {
                        validarCadString = '1';
                    }
                    else
                    {

                        dataGridView1.Rows.Add(unirString + "\"", "CADENA", contLineas, contColumnas);
                        //contar_lineas += 1;
                        validarCadString = '0';
                        unirString = "";


                    }
                }

                if (validarCadString.Equals('1'))
                {
                    unirString = unirString + letra2;
                }


                if (Regex.IsMatch(letra2, coment))
                {
                    validarCom = '1';
                }

                if (validarCom.Equals('1'))
                {
                    unirCom = unirCom + letra2;

                    if (letra.Equals('\n'))
                    {

                        dataGridView1.Rows.Add(unirCom + "", "COMENTARIO", contLineas, contColumnas);
                        contLineas += 1;
                        contColumnas = 1;
                        validarCom = '0';
                        unirCom = "";


                    }

                }

                else if (validarCom.Equals('0') & validarCadString.Equals('0') & letra2 != "\"" & letra2 != "\r")
                {
                    if (letra2 == " " || letra2 == "\n")
                    {
                        this.AnalizarPalabras();

                        if (Regex.IsMatch(letra2, espacio))
                        {
                            contColumnas += 1;
                        }
                        if (Regex.IsMatch(letra2, saltoLinea))
                        {
                            contLineas += 1;
                            contColumnas = 1;
                        }
                    }
                    else
                    {
                        unir = unir + letra2;
                    }

                }


            }

        }


        public void AnalizarPalabras()
        {


            string minusculas = "[A-Z]+";

            if (Regex.IsMatch(unir, minusculas))
            {
                dataGridView1.Rows.Add(unir + "", "ERROR", contLineas, contColumnas);
                unir = "";
            }
            else
            {
                this.VerificarLexema();
            }
        }


        char validarDupl = '0';
        public void VerificarLexema()
        {


            string[] reservado = { "inicio", "proceso", "fin", "si", "ver", "mientras", "entero", "cadena" };


            string numeros = "^[0-9]+$[0-9]?";
            string delimitadores = "^[;|(|)|{|}]$";
            string operadores = "^[+|-|/|*]$";
            string asignacion = "^#$";
            string comparadores = "^[<|>]$|^==$";
            string variables = "^var[(0-9)?]$";


            char validarReservado = '0';



            for (int i = 0; i < 8; i++)
            {
                if (unir.Equals(reservado[i]))
                {
                    dataGridView1.Rows.Add(unir + "", "RESERVADO", contLineas, contColumnas);
                    validarReservado = '1';
                    if (Regex.IsMatch(unir, "si"))
                    {
                        validarDupl = '1';
                    }
                }

            }

            if (Regex.IsMatch(unir, numeros))
            {
                dataGridView1.Rows.Add(unir + "", "NUMERO", contLineas, contColumnas);
            }
            else if (Regex.IsMatch(unir, delimitadores))
            {
                dataGridView1.Rows.Add(unir + "", "DELIMITADOR", contLineas, contColumnas);

            }
            else if (Regex.IsMatch(unir, operadores))
            {
                dataGridView1.Rows.Add(unir + "", "OPERADOR", contLineas, contColumnas);
            }
            else if (Regex.IsMatch(unir, asignacion))
            {
                dataGridView1.Rows.Add(unir + "", "ASIGNACION", contLineas, contColumnas);
            }
            else if (Regex.IsMatch(unir, comparadores))
            {
                dataGridView1.Rows.Add(unir + "", "COMPARADOR", contLineas, contColumnas);
            }
            else if (Regex.IsMatch(unir, variables))
            {
                dataGridView1.Rows.Add(unir + "", "VARIABLE", contLineas, contColumnas);
            }

            else if (validarReservado.Equals('0') & unir != "" & unir != "\"")
            {

                dataGridView1.Rows.Add(unir + "", "ERROR", contLineas, contColumnas);

            }
            unir = "";
        }

        int recorrido = 0;
        string[] lexSint = null;
        string[] numLinea = null;
        string[] numColumna = null;


        string[] variables = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        int contVariables = 0;
        string[] tipoVariables = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        string inicioCorrec = "0";
        string procesoCorrec = "0";
        string existPrintNoExistProcs = "0";


        string existProcs = "0";
        string existFin = "0";

        string[] pasarAC = null;
        int pasosPasarAC = 0;

        public void AnalisisSintactico()
        {


            pasarAC = new string[30];


            int contComentarios = 0;
            int contComentarios_filaMenos = 0;

            for (int x = 0; x < dataGridView1.RowCount; x++)
            {
                if ((dataGridView1.Rows[x].Cells[1].Value.ToString()).Equals("COMENTARIO"))
                {
                    contComentarios_filaMenos += 1;

                }
            }

            if (contComentarios_filaMenos > 0)
            {
                lexSint = new string[(dataGridView1.RowCount - contComentarios_filaMenos) + 1];
                numLinea = new string[(dataGridView1.RowCount - contComentarios_filaMenos) + 1];
                numColumna = new string[(dataGridView1.RowCount - contComentarios_filaMenos) + 1];






                for (int s = 0; s <= (dataGridView1.RowCount - 1); s++)
                {
                    if ((dataGridView1.Rows[s].Cells[1].Value.ToString()).Equals("COMENTARIO"))
                    {
                        contComentarios += 1;

                    }
                    else
                    {
                        lexSint[s - contComentarios] = dataGridView1.Rows[s].Cells[0].Value.ToString();
                        numLinea[s - contComentarios] = dataGridView1.Rows[s].Cells[2].Value.ToString();
                        numColumna[s - contComentarios] = dataGridView1.Rows[s].Cells[3].Value.ToString();
                    }

                }




            }
            else
            {
                lexSint = new string[(dataGridView1.RowCount - contComentarios_filaMenos) + 1];
                numLinea = new string[(dataGridView1.RowCount - contComentarios_filaMenos) + 1];
                numColumna = new string[(dataGridView1.RowCount - contComentarios_filaMenos) + 1];

                for (int s = 0; s < (dataGridView1.RowCount - contComentarios_filaMenos); s++)
                {
                    if ((dataGridView1.Rows[s].Cells[1].Value.ToString()).Equals("COMENTARIO"))
                    {
                        contComentarios += 1;
                    }
                    else
                    {
                        lexSint[s - contComentarios] = dataGridView1.Rows[s].Cells[0].Value.ToString();
                        numLinea[s - contComentarios] = dataGridView1.Rows[s].Cells[2].Value.ToString();
                        numColumna[s - contComentarios] = dataGridView1.Rows[s].Cells[3].Value.ToString();
                    }

                }
            }

            lexSint[dataGridView1.RowCount - contComentarios_filaMenos] = "ultima linea";
            numLinea[dataGridView1.RowCount - contComentarios_filaMenos] = "ultima linea";
            numColumna[dataGridView1.RowCount - contComentarios_filaMenos] = "ultima linea";


            for (int recorridox = 0; recorridox < lexSint.Length; recorridox++)
            {

                if (Regex.IsMatch(lexSint[recorridox], "inicio"))
                {
                    recorridox += 1;
                    if (Regex.IsMatch(lexSint[recorridox], ";"))
                    {

                        recorridox += 1;
                        //inicio_hay = "1";
                    }

                }

                if (Regex.IsMatch(lexSint[recorridox], "proceso"))
                {
                    recorridox += 1;
                    if (Regex.IsMatch(lexSint[recorridox], ";"))
                    {
                        recorridox += 1;
                        existProcs = "1";
                    }

                }

                if (Regex.IsMatch(lexSint[recorridox], "fin"))
                {
                    recorridox += 1;
                    if (Regex.IsMatch(lexSint[recorridox], ";"))
                    {
                        recorridox += 1;
                        existFin = "1";
                    }

                }
            }


            for (recorrido = 0; recorrido < lexSint.Length; recorrido++)
            {

                if (Regex.IsMatch(lexSint[recorrido], "inicio"))
                {
                    recorrido += 1;
                    if (Regex.IsMatch(lexSint[recorrido], ";"))
                    {

                        dataGridView2.Rows.Add("CORRECTA", "INICIALIZACION --> inicio ; <-- " + " FILA: " + numLinea[recorrido]);
                        recorrido += 1;
                        inicioCorrec = "1";
                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA PUNTO Y COMA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido]);
                    }
                }

                if (inicioCorrec == "0")
                {
                    if (recorrido == 0)
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA INICIALIZACION: --> inicio ; <-- " + "ANTES DE FILA: " + numLinea[recorrido]);

                    }
                    //Console.Write("SE ESPERABA INICIALIZACION: inicio ;" + "antes de fila: " + num_linea[recorrido] +"\n");
                    recorrido_sum = recorrido;
                    this.estruc_var_cadena();
                    recorrido = recorrido_sum;

                    recorrido_sum = recorrido;
                    this.estruc_var_entera();
                    recorrido = recorrido_sum;
                }
                if (inicioCorrec == "1")
                {
                    recorrido_sum = recorrido;
                    this.estruc_var_cadena();


                    recorrido_sum = recorrido;
                    this.estruc_var_entera();

                }


                if (Regex.IsMatch(lexSint[recorrido], "proceso"))
                {

                    recorrido += 1;
                    if (Regex.IsMatch(lexSint[recorrido], ";"))
                    {

                        dataGridView2.Rows.Add("CORRECTA", "INICIALIZACION --> proceso ; <-- " + "FILA: " + numLinea[recorrido]);

                        recorrido += 1;
                        procesoCorrec = "1";
                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA PUNTO Y COMA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido]);
                    }
                }


                if (procesoCorrec == "1")
                {
                    recorrido_sum = recorrido;
                    this.estruc_ver();
                    recorrido = recorrido_sum;

                    recorrido_sum = recorrido;
                    this.estruc_si();
                    recorrido = recorrido_sum;

                    recorrido_sum = recorrido;
                    this.estruc_mientras();
                    recorrido = recorrido_sum;

                    recorrido_sum = recorrido;
                    this.estruc_var_cadenax();


                    recorrido_sum = recorrido;
                    this.estruc_var_enterax();
                }

                if (existProcs == "1" & procesoCorrec == "0")
                {

                    recorrido_sum = recorrido;
                    this.estruc_verx();
                    recorrido = recorrido_sum;

                    recorrido_sum = recorrido;
                    this.estruc_six();
                    recorrido = recorrido_sum;

                    recorrido_sum = recorrido;
                    this.estruc_mientrasx();
                    recorrido = recorrido_sum;

                }
                else if (procesoCorrec == "0")
                {
                    recorrido_sum = recorrido;
                    this.estruc_ver();
                    recorrido = recorrido_sum;

                    recorrido_sum = recorrido;
                    this.estruc_si();
                    recorrido = recorrido_sum;

                    recorrido_sum = recorrido;
                    this.estruc_mientras();
                    recorrido = recorrido_sum;
                }


                if (Regex.IsMatch(lexSint[recorrido], "fin"))
                {

                    recorrido += 1;
                    if (Regex.IsMatch(lexSint[recorrido], ";"))
                    {
                        existFin = "1";
                        dataGridView2.Rows.Add("CORRECTA", "FINALIZACION --> fin ; <--" + " FILA: " + numLinea[recorrido]);
                        recorrido += 1;

                    }
                    else
                    {
                        //  dataGridView2.Rows.Add("ERROR", "SE ESPERABA PUNTO Y COMA" + " FILA: " + num_linea[recorrido] + " COLUMNA: " + num_columna[recorrido]);
                    }
                }

                if (Regex.IsMatch(lexSint[recorrido], "ultima linea"))
                {
                    if (lexSint[recorrido - 2].Equals("fin") & lexSint[recorrido - 1].Equals(";"))
                    {
                        //  dataGridView2.Rows.Add("CORRECTA", "FINALIZACION --> fin ; <--" + " FILA: " + num_linea[recorrido]);
                    }
                    else
                    {
                        if (existFin == "1")
                        {
                            dataGridView2.Rows.Add("ERROR", "NO PUEDE ESCRIBIR DESPUES DE: --> fin ; <--" + " " + numLinea[recorrido]);

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "SE ESPERABA FINALIZACION: --> fin ; <--" + " " + numLinea[recorrido]);

                        }
                    }
                }




            }



        }


        int contErrorCompilar = 0;

        int recorrido_sum = 0;
        public void estruc_var_entera()
        {

            string existe = "no";
            if (Regex.IsMatch(lexSint[recorrido_sum], "entero"))
            {
                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], "#"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexSint[recorrido_sum], "^[0-9]+$[0-9]?"))
                        {
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexSint[recorrido_sum], ";"))
                            {
                                for (int i = 0; i < contVariables + 1; i++)
                                {
                                    if (variables[i].Equals(lexSint[recorrido_sum - 3]))
                                    {
                                        existe = "si";
                                    }
                                }
                                if (existe == "si")
                                {
                                    dataGridView2.Rows.Add("ERROR", "NOMBRE DE VARIABLE YA EXISTE" + " FILA: " + numLinea[recorrido]);
                                }
                                if (existe == "no")
                                {
                                    variables[contVariables] = lexSint[recorrido_sum - 3];
                                    tipoVariables[contVariables] = "numero";
                                    contVariables += 1;
                                    dataGridView2.Rows.Add("CORRECTA", "ASIGNACION DE VARIABLE ENTERA" + " FILA: " + numLinea[recorrido]);

                                    pasarAC[pasosPasarAC] = "int " + lexSint[recorrido_sum - 3] + " = " + lexSint[recorrido_sum - 1] + ";" + "\n";
                                    pasosPasarAC += 1;
                                }



                            }
                            else
                            {

                                dataGridView2.Rows.Add("ERROR", "SE ESPERABA PUNTO Y COMA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                            }

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "SE ESPERABA UN NUMERO" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                        }

                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA ASIGNADOR" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }
                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA VARIABLE" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }
                recorrido = recorrido_sum;
            }
            //finaliza reconocimento de declaracion de variable entera
        } //fin public void estruc_var_numero




        public void estruc_var_enterax()
        {

            //inicia reconocimento de declaracion de variable enterax

            if (Regex.IsMatch(lexSint[recorrido_sum], "entero"))
            {
                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], "#"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexSint[recorrido_sum], "^[0-9]+$[0-9]?"))
                        {
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexSint[recorrido_sum], ";"))
                            {
                                dataGridView2.Rows.Add("ERROR", "DECLARACION --> entero <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]);
                            }
                            else
                            {

                                dataGridView2.Rows.Add("ERROR", "DECLARACION --> entero <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                            }

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "DECLARACION --> entero <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                        }

                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "DECLARACION --> entero <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                    }
                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "DECLARACION --> entero <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                }
                recorrido = recorrido_sum;
            }

        }

        public void estruc_var_cadena()
        {

            string existe = "no";
            if (Regex.IsMatch(lexSint[recorrido_sum], "cadena"))
            {
                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], "#"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                        {
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexSint[recorrido_sum], ";"))
                            {
                                for (int i = 0; i < contVariables + 1; i++)
                                {
                                    if (variables[i].Equals(lexSint[recorrido_sum - 3]))
                                    {
                                        existe = "si";
                                    }
                                }
                                if (existe == "si")
                                {
                                    dataGridView2.Rows.Add("ERROR", "NOMBRE DE VARIABLE YA EXISTE" + " FILA: " + numLinea[recorrido]);
                                }
                                if (existe == "no")
                                {
                                    variables[contVariables] = lexSint[recorrido_sum - 3];
                                    tipoVariables[contVariables] = "cadena";
                                    contVariables += 1;
                                    dataGridView2.Rows.Add("CORRECTA", "ASIGNACION DE VARIABLE CADENA" + " FILA: " + numLinea[recorrido]);
                                    pasarAC[pasosPasarAC] = "string " + lexSint[recorrido_sum - 3] + " = " + lexSint[recorrido_sum - 1] + ";" + "\n";
                                    pasosPasarAC += 1;
                                }
                            }
                            else
                            {
                                dataGridView2.Rows.Add("ERROR", "SE ESPERABA PUNTO Y COMA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                            }

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "SE ESPERABA UNA CADENA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                        }

                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA ASIGNADOR" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }
                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA VARIABLE" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }
                recorrido = recorrido_sum;
            }

        }



        public void estruc_var_cadenax()
        {

            if (Regex.IsMatch(lexSint[recorrido_sum], "cadena"))
            {
                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], "#"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                        {
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexSint[recorrido_sum], ";"))
                            {
                                dataGridView2.Rows.Add("ERROR", "DECLARACION --> cadena <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]);
                            }
                            else
                            {
                                dataGridView2.Rows.Add("ERROR", "DECLARACION --> cadena <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                            }

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "DECLARACION --> cadena <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                        }

                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "DECLARACION --> cadena <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                    }
                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "DECLARACION --> cadena <--DEBE IR ANTES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                }
                recorrido = recorrido_sum;
            }

        }




        public void estruc_ver()
        {

            if (Regex.IsMatch(lexSint[recorrido_sum], "ver"))
            {
                if ((lexSint[recorrido_sum - 2] != "proceso" || lexSint[recorrido_sum - 1] != "proceso") & procesoCorrec == "0" & existPrintNoExistProcs == "0")
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA INICIALIZACION: --> proceso ; <--" + " ANTES DE FILA: " + numLinea[recorrido] + "\n");
                    existPrintNoExistProcs = "1";
                }
                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], ";"))
                    {
                        dataGridView2.Rows.Add("CORRECTA", "IMPRIMIR EN PANTALLA VER" + " FILA: " + numLinea[recorrido_sum]);
                        pasarAC[pasosPasarAC] = " cout << " + lexSint[recorrido_sum - 1] + " << endl" + ";" + "\n";
                        pasosPasarAC += 1;
                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA PUNTO Y COMA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }

                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA UNA CADENA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }

        }



        public void estruc_verx()
        {

            if (Regex.IsMatch(lexSint[recorrido_sum], "ver"))
            {
                //if ((lexi_a_sint[recorrido_sum - 2] != "proceso" || lexi_a_sint[recorrido_sum - 1] != "proceso") & proceso_correcto == "0" & ya_imprimio_no_hay_proc == "0")
                //{
                //    dataGridView2.Rows.Add("ERROR", "SE ESPERABA INICIALIZACION: --> proceso ; <--" + " ANTES DE FILA: " + num_linea[recorrido] + "\n");
                //    ya_imprimio_no_hay_proc = "1";
                //}
                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], ";"))
                    {
                        dataGridView2.Rows.Add("ERROR", "ESTRUCTURA --> ver <--DEBE IR DESPUES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]);
                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "ESTRUCTURA --> ver <--DEBE IR DESPUES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                    }

                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "ESTRUCTURA --> ver <--DEBE IR DESPUES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido]); recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }
            // recorrido_sum -= 1;
        }

        public void estruc_si()
        {

            if (Regex.IsMatch(lexSint[recorrido_sum], "si"))
            {
                if ((lexSint[recorrido_sum - 2] != "proceso" || lexSint[recorrido_sum - 1] != "proceso") & procesoCorrec == "0" & existPrintNoExistProcs == "0")
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA INICIALIZACION: --> proceso ; <--" + " ANTES DE FILA: " + numLinea[recorrido_sum]);
                    existPrintNoExistProcs = "1";
                }
                recorrido_sum += 1;
                if (lexSint[recorrido_sum].Equals("("))
                {
                    recorrido_sum += 1;
                    this.estruc_comparacion();
                    recorrido_sum += 1;
                    if (lexSint[recorrido_sum].Equals(")"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexSint[recorrido_sum], "{"))
                        {
                            recorrido_sum += 1;
                            this.estruc_ver_dentro_de_si();
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexSint[recorrido_sum], "}"))
                            {
                                dataGridView2.Rows.Add("CORRECTA", "SI ( ) { }" + " FINALIZACION FILA: " + numLinea[recorrido_sum]);
                                //pasar_a_c[pasos_pasar_a_c] = " if ( ";

                                //pasos_pasar_a_c += 1;
                            }
                            else
                            {
                                dataGridView2.Rows.Add("ERROR", "SE ESPERABA CIERRE DE LLAVE" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                            }

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "SE ESPERABA APERTURA DE LLAVE" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                        }

                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA CIERRE DE PARENTESIS" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }
                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA APERTURA DE PARENTESIS" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }

        }





        public void estruc_six()
        {

            if (Regex.IsMatch(lexSint[recorrido_sum], "si"))
            {
                //if ((lexi_a_sint[recorrido_sum - 2] != "proceso" || lexi_a_sint[recorrido_sum - 1] != "proceso") & proceso_correcto == "0" & ya_imprimio_no_hay_proc == "0")
                //{
                //    dataGridView2.Rows.Add("ERROR", "SE ESPERABA INICIALIZACION: --> proceso ; <--" + " ANTES DE FILA: " + num_linea[recorrido_sum]);
                //    ya_imprimio_no_hay_proc = "1";
                //}
                recorrido_sum += 1;
                if (lexSint[recorrido_sum].Equals("("))
                {
                    recorrido_sum += 1;
                    this.estruc_comparacion();
                    recorrido_sum += 1;
                    if (lexSint[recorrido_sum].Equals(")"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexSint[recorrido_sum], "{"))
                        {
                            recorrido_sum += 1;
                            this.estruc_ver_dentro_de_si();
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexSint[recorrido_sum], "}"))
                            {
                                dataGridView2.Rows.Add("ERROR", "ESTRUCTURA --> si <--DEBE IR DESPUES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido_sum]);
                            }
                            else
                            {
                                dataGridView2.Rows.Add("ERROR", "ESTRUCTURA --> si <--DEBE IR DESPUES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido_sum]); recorrido_sum -= 1;
                            }

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "ESTRUCTURA --> si <--DEBE IR DESPUES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido_sum]); recorrido_sum -= 1;
                        }

                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "ESTRUCTURA --> si <--DEBE IR DESPUES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido_sum]); recorrido_sum -= 1;
                    }
                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "ESTRUCTURA --> si <--DEBE IR DESPUES DE --> proceso ; <-- " + "FILA: " + numLinea[recorrido_sum]); recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }

        }


        public void estruc_mientras()
        {
            //inicia reconocimento de mientras

            if (Regex.IsMatch(lexSint[recorrido_sum], "mientras"))
            {
                if ((lexSint[recorrido_sum - 2] != "proceso" || lexSint[recorrido_sum - 1] != "proceso") & procesoCorrec == "0" & existPrintNoExistProcs == "0")
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA INICIALIZACION: --> proceso ; <--" + " ANTES DE FILA: " + numLinea[recorrido_sum]);
                    existPrintNoExistProcs = "1";
                }
                recorrido_sum += 1;
                if (lexSint[recorrido_sum].Equals("("))
                {
                    recorrido_sum += 1;
                    this.estruc_comparacion_mientras();
                    recorrido_sum += 1;
                    if (lexSint[recorrido_sum].Equals(")"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexSint[recorrido_sum], "{"))
                        {
                            recorrido_sum += 1;
                            this.estruc_ver_dentro_de_si();
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexSint[recorrido_sum], "}"))
                            {
                                dataGridView2.Rows.Add("CORRECTA", "MIENTRAS ( ) { }" + " FINALIZACION FILA: " + numLinea[recorrido_sum]);
                            }
                            else
                            {
                                dataGridView2.Rows.Add("ERROR", "SE ESPERABA CIERRE DE LLAVE" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                            }

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "SE ESPERABA APERTURA DE LLAVE" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                        }

                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA CIERRE DE PARENTESIS" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }
                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA APERTURA DE PARENTESIS" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }

        }


        public void estruc_mientrasx()
        {

            if (Regex.IsMatch(lexSint[recorrido_sum], "mientras"))
            {
                //if ((lexi_a_sint[recorrido_sum - 2] != "proceso" || lexi_a_sint[recorrido_sum - 1] != "proceso") & proceso_correcto == "0" & ya_imprimio_no_hay_proc == "0")
                //{
                //    dataGridView2.Rows.Add("ERROR", "SE ESPERABA INICIALIZACION: --> proceso ; <--" + " ANTES DE FILA: " + num_linea[recorrido_sum]);
                //    ya_imprimio_no_hay_proc = "1";
                //}
                recorrido_sum += 1;
                if (lexSint[recorrido_sum].Equals("("))
                {
                    recorrido_sum += 1;
                    this.estruc_comparacion_mientras();
                    recorrido_sum += 1;
                    if (lexSint[recorrido_sum].Equals(")"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexSint[recorrido_sum], "{"))
                        {
                            recorrido_sum += 1;
                            this.estruc_ver_dentro_de_si();
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexSint[recorrido_sum], "}"))
                            {
                                dataGridView2.Rows.Add("CORRECTA", "MIENTRAS ( ) { }" + " FINALIZACION FILA: " + numLinea[recorrido_sum]);
                            }
                            else
                            {
                                dataGridView2.Rows.Add("ERROR", "SE ESPERABA CIERRE DE LLAVE" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                            }

                        }
                        else
                        {
                            dataGridView2.Rows.Add("ERROR", "SE ESPERABA APERTURA DE LLAVE" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                        }

                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA CIERRE DE PARENTESIS" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }
                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA APERTURA DE PARENTESIS" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }

        }


        public void estruc_ver_dentro_de_si()
        {

            if (Regex.IsMatch(lexSint[recorrido_sum], "ver"))
            {
                if ((lexSint[recorrido_sum - 2] != "proceso" || lexSint[recorrido_sum - 1] != "proceso") & procesoCorrec == "0" & existPrintNoExistProcs == "0")
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA INICIALIZACION: --> proceso ; <--" + " ANTES DE FILA: " + numLinea[recorrido_sum]);
                    existPrintNoExistProcs = "1";
                }
                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], ";"))
                    {
                        dataGridView2.Rows.Add("CORRECTA", "IMPRIMIR EN PANTALLA VER" + " FILA: " + numLinea[recorrido_sum]);
                        pasarAC[pasosPasarAC] = "{ " + "\n";
                        pasosPasarAC += 1;
                        pasarAC[pasosPasarAC] = " cout << " + lexSint[recorrido_sum - 1] + " << endl" + ";" + "\n";
                        pasosPasarAC += 1;
                        pasarAC[pasosPasarAC] = "} " + "\n";
                        pasosPasarAC += 1;
                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA PUNTO Y COMA" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }

                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA UNA CADENA" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }
            else
            {
                pasarAC[pasosPasarAC] = "{ " + "\n";
                pasosPasarAC += 1;

                pasarAC[pasosPasarAC] = "} " + "\n";
                pasosPasarAC += 1;
                recorrido_sum -= 1;
            }


        }

        public void estruc_comparacion()
        {

            string existe = "no";
            string existe2 = "no";
            string tipo = "ninguno";
            string tipo2 = "ninguno";

            if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$|^[0-9]+$[0-9]?|^\".*\"$"))
            {
                if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    for (int i = 0; i < contVariables + 1; i++)
                    {
                        if (variables[i].Equals(lexSint[recorrido_sum]))
                        {
                            tipo = tipoVariables[i];
                        }
                    }
                }
                else if (Regex.IsMatch(lexSint[recorrido_sum], "^[0-9]+$[0-9]?"))
                {
                    tipo = "numero";
                    existe = "si";
                }
                else if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                {
                    tipo = "cadena";
                    existe = "si";
                }


                if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    for (int i = 0; i < contVariables + 1; i++)
                    {
                        if (variables[i].Equals(lexSint[recorrido_sum]))
                        {
                            existe = "si";
                        }
                    }
                    if (existe == "si")
                    {

                    }
                    if (existe == "no")
                    {
                        dataGridView2.Rows.Add("ERROR", "NOMBRE DE VARIABLE NO DECLARADA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]);
                    }
                }



                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^[<|>]$|^==$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$|^[0-9]+$[0-9]?|^\".*\"$"))
                    {
                        if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                        {
                            for (int i = 0; i < contVariables + 1; i++)
                            {
                                if (variables[i].Equals(lexSint[recorrido_sum]))
                                {
                                    tipo2 = tipoVariables[i];
                                }
                            }
                        }
                        else if (Regex.IsMatch(lexSint[recorrido_sum], "^[0-9]+$[0-9]?"))
                        {
                            tipo2 = "numero";
                            existe2 = "si";
                        }
                        else if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                        {
                            tipo2 = "cadena";
                            existe2 = "si";
                        }

                        if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                        {
                            for (int i = 0; i < contVariables + 1; i++)
                            {
                                if (variables[i].Equals(lexSint[recorrido_sum]))
                                {
                                    existe2 = "si";
                                }
                            }
                            if (existe2 == "si")
                            {

                            }
                            if (existe2 == "no")
                            {
                                dataGridView2.Rows.Add("ERROR", "NOMBRE DE VARIABLE NO DECLARADA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]);
                            }
                        }

                        if (existe == "no" || existe2 == "no")
                        {

                        }
                        else
                        {

                            if (tipo == tipo2)
                            {
                                dataGridView2.Rows.Add("CORRECTA", " COMPARACION" + " FILA: " + numLinea[recorrido_sum]);
                                pasarAC[pasosPasarAC] = "if ( " + lexSint[recorrido_sum - 2] + " " + lexSint[recorrido_sum - 1] + " " + lexSint[recorrido_sum] + " )" + "\n";
                                pasosPasarAC += 1;

                            }
                            else
                            {
                                dataGridView2.Rows.Add("ERROR", " DATOS DIFERENTES EN COMPARACION " + " FILA: " + numLinea[recorrido_sum]);
                            }


                        }
                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA VARIABLE, CADENA O NUMERO" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }

                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA UN COMPARADOR" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }
            }
            else
            {
                dataGridView2.Rows.Add("ERROR", "SE ESPERABA UNA VARIABLE, CADENA O NUMERO" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
            }

        }





        public void estruc_comparacion_mientras()
        {
            string existe = "no";
            string existe2 = "no";
            string tipo = "ninguno";
            string tipo2 = "ninguno";

            if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$|^[0-9]+$[0-9]?|^\".*\"$"))
            {
                if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    for (int i = 0; i < contVariables + 1; i++)
                    {
                        if (variables[i].Equals(lexSint[recorrido_sum]))
                        {
                            tipo = tipoVariables[i];
                        }
                    }
                }
                else if (Regex.IsMatch(lexSint[recorrido_sum], "^[0-9]+$[0-9]?"))
                {
                    tipo = "numero";
                    existe = "si";
                }
                else if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                {
                    tipo = "cadena";
                    existe = "si";
                }


                if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    for (int i = 0; i < contVariables + 1; i++)
                    {
                        if (variables[i].Equals(lexSint[recorrido_sum]))
                        {
                            existe = "si";
                        }
                    }
                    if (existe == "si")
                    {

                    }
                    if (existe == "no")
                    {
                        dataGridView2.Rows.Add("ERROR", "NOMBRE DE VARIABLE NO DECLARADA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]);
                    }
                }



                recorrido_sum += 1;
                if (Regex.IsMatch(lexSint[recorrido_sum], "^[<|>]$|^==$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$|^[0-9]+$[0-9]?|^\".*\"$"))
                    {
                        if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                        {
                            for (int i = 0; i < contVariables + 1; i++)
                            {
                                if (variables[i].Equals(lexSint[recorrido_sum]))
                                {
                                    tipo2 = tipoVariables[i];
                                }
                            }
                        }
                        else if (Regex.IsMatch(lexSint[recorrido_sum], "^[0-9]+$[0-9]?"))
                        {
                            tipo2 = "numero";
                            existe2 = "si";
                        }
                        else if (Regex.IsMatch(lexSint[recorrido_sum], "^\".*\"$"))
                        {
                            tipo2 = "cadena";
                            existe2 = "si";
                        }

                        if (Regex.IsMatch(lexSint[recorrido_sum], "^var[(0-9)?]$"))
                        {
                            for (int i = 0; i < contVariables + 1; i++)
                            {
                                if (variables[i].Equals(lexSint[recorrido_sum]))
                                {
                                    existe2 = "si";
                                }
                            }
                            if (existe2 == "si")
                            {

                            }
                            if (existe2 == "no")
                            {
                                dataGridView2.Rows.Add("ERROR", "NOMBRE DE VARIABLE NO DECLARADA" + " FILA: " + numLinea[recorrido] + " COLUMNA: " + numColumna[recorrido_sum]);
                            }
                        }

                        if (existe == "no" || existe2 == "no")
                        {

                        }
                        else
                        {

                            if (tipo == tipo2)
                            {
                                dataGridView2.Rows.Add("CORRECTA", " COMPARACION" + " FILA: " + numLinea[recorrido_sum]);
                                pasarAC[pasosPasarAC] = "while ( " + lexSint[recorrido_sum - 2] + " " + lexSint[recorrido_sum - 1] + " " + lexSint[recorrido_sum] + " )" + "\n";
                                pasosPasarAC += 1;

                            }
                            else
                            {
                                dataGridView2.Rows.Add("ERROR", " DATOS DIFERENTES EN COMPARACION " + " FILA: " + numLinea[recorrido_sum]);
                            }


                        }
                    }
                    else
                    {
                        dataGridView2.Rows.Add("ERROR", "SE ESPERABA VARIABLE, CADENA O NUMERO" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                    }

                }
                else
                {
                    dataGridView2.Rows.Add("ERROR", "SE ESPERABA UN COMPARADOR" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
                }

                // recorrido = recorrido_sum;
            }
            else
            {
                dataGridView2.Rows.Add("ERROR", "SE ESPERABA UNA VARIABLE, CADENA O NUMERO" + " FILA: " + numLinea[recorrido_sum] + " COLUMNA: " + numColumna[recorrido_sum]); recorrido_sum -= 1;
            }

        }

    }    
}
