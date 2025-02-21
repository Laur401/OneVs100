using System;
using Avalonia.Controls;
using OneVs100.CustomControls;
using OneVs100.Helpers;

namespace OneVs100.ViewModels;

public class MobMember
{
    private int number;
    private float intelligence;
    private char answer = ' ';
    public bool isKnockedOut = false;
    RandomGaussian RNG = new();

    public MobMember(int number)
    {
        this.number = number;
        this.intelligence = (float)RNG.BoxMuller(0, 36);
    }

    public void selectAnswer(char correctAnswer, float difficulty, int questionNumber)
    {
        this.answer = questionNumber * difficulty < intelligence
            ? correctAnswer
            : RNG.GetItems(['a', 'b', 'c'], 1)[0];
    }

    public bool isAnswerCorrect(char correctAnswer)
    {
        if (correctAnswer == answer)
            return true;
        else return false;
    }


    //Knock-out calculation - Difficulty*QuestionNr > Intelligence
}