﻿using System;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;
using System.Numerics;
using System.Collections.Generic;
using MathNet.Filtering;
using MathNet.Filtering.IIR;

//public class Filter2
//{
//    private double[] butterworthCoefficients;
//    private int FFT_size;
//    private Complex[] fftSignal;

//    public Filter2(double lowCut = 45.0, double highCut = 55.0, double frequency = 250.0, int n = 1024)
//    {
//        FFT_size = n;
//        butterworthCoefficients = new double[FFT_size / 2];
//        fftSignal = new Complex[FFT_size];
//        InitializeButterworthGain(lowCut, highCut, frequency);
//    }

//    private void InitializeButterworthGain(double lowCut, double highCut, double fs)
//    {
//        for (int i = 0; i < FFT_size / 2; i++)
//        {
//            double frequency = i * fs / FFT_size;
//            if (frequency < lowCut || frequency > highCut)
//                butterworthCoefficients[i] = 0;
//            else
//                // Применяем формулу Баттерворта
//                butterworthCoefficients[i] = 1 / Math.Sqrt(1 + Math.Pow((frequency - (highCut + lowCut) / 2) / ((highCut - lowCut) / 2), 4));
//        }
//    }

//    public double ApplyFilter(double data)
//    {
//        // Перемещаем старые значения для создания кольцевого буфера
//        for (int i = 0; i < FFT_size - 1; i++)
//        {
//            fftSignal[i] = fftSignal[i + 1];
//        }
//        fftSignal[FFT_size - 1] = new Complex(data, 0);

//        // Прямое FFT-преобразование
//        Fourier.Forward(fftSignal, FourierOptions.Default);

//        // Применяем коэффициенты усиления Баттерворта к каждой частоте
//        for (int w = 0; w < FFT_size / 2; w++)
//        {
//            fftSignal[w] *= butterworthCoefficients[w];
//            fftSignal[FFT_size - w - 1] *= butterworthCoefficients[w]; // Симметрия для зеркальной части спектра
//        }

//        // Обратное FFT-преобразование
//        Fourier.Inverse(fftSignal, FourierOptions.Default);

//        // Возвращаем отфильтрованное значение
//        return fftSignal[0].Real;
//    }
//}

public class Filter
{

    private readonly MathNet.Filtering.IIR.OnlineIirFilter filter;

    public Filter(double lowCut = 45.0, double highCut = 55.0, double fs = 250.0)
    {
        //filter = new IirButterworthBandpass(4, fs, lowCut, highCut);

        var passBandFreq = 4 / (fs / 2.0);
        var stopBandFreq = 1 / (fs / 2.0);
        var passBandRipple = 1.0;
        var stopBandAttenuation = 20.0;
        var coefficients = MathNet.Filtering.Butterworth.IirCoefficients.HighPass(stopBandFreq, passBandFreq, passBandRipple, stopBandAttenuation);

        var coeffs = new List<double>();
        foreach (var numerator in coefficients.numerator)
        {
            coeffs.Add(numerator);
        }
        foreach (var denominator in coefficients.denominator)
        {
            coeffs.Add(denominator);
        }
        filter = new MathNet.Filtering.IIR.OnlineIirFilter(coeffs.ToArray());
        //var filteredSignal = filter.ProcessSamples(signal);

    }

    public double ApplyFilter(double data)
    {
        return filter.ProcessSample(data);
    }
}