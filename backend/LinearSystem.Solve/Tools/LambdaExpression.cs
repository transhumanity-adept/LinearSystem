namespace LinearSystem.Solve.Tools;

public struct LambdaExpression
{
    private List<LambdaDigit> _digits;
    public List<LambdaDigit> LambdaDigits => _digits.ToList();
    public LambdaExpression(IEnumerable<LambdaDigit> digits)
    {
        _digits = digits.ToList();
        Simplification();
    }
    public LambdaExpression(LambdaDigit digit)
    {
        _digits = new List<LambdaDigit>() {digit};
    }
    private void Simplification()
    {
        _digits = _digits
            .GroupBy(digits => digits.LambdaDegree)
            .Select(group =>
                new LambdaDigit(group.Aggregate(0d, (sum, currentDigit) => sum += currentDigit.Coefficient),
                    group.First().LambdaDegree))
            .ToList();
    }

    public static LambdaExpression operator +(LambdaExpression expression_one, LambdaExpression expression_two)
    {
        List<LambdaDigit> new_digits = new List<LambdaDigit>();
        foreach (LambdaDigit digit in expression_one._digits)
        {
            List<LambdaDigit> digits_with_equal_lambda =
                expression_two._digits.Where(d => d.LambdaDegree == digit.LambdaDegree).ToList();
            double new_coefficient = digit.Coefficient +
                                     digits_with_equal_lambda.Aggregate(0d,
                                         (sum, current_digit) => sum += current_digit.Coefficient);
            new_digits.Add(new LambdaDigit(new_coefficient, digit.LambdaDegree));
        }

        new_digits.AddRange(expression_two._digits.Where(digit_two =>
            !expression_one._digits.Any(digit_one => digit_one.LambdaDegree == digit_two.LambdaDegree)));
        return new LambdaExpression(new_digits);
    }

    public static LambdaExpression operator -(LambdaExpression expression)
    {
        return new LambdaExpression(expression._digits.Select(digit =>
            new LambdaDigit(-digit.Coefficient, digit.LambdaDegree)));
    }

    public static LambdaExpression operator -(LambdaExpression expression_one, LambdaExpression expression_two)
    {
        return expression_one + -expression_two;
    }

    public static LambdaExpression operator *(LambdaExpression expression_one, LambdaExpression expression_two)
    {
        List<LambdaDigit> new_digits = expression_one._digits
            .Select(digit_expression_one => expression_two._digits
                .Select(digit_expression_two =>
                    new LambdaDigit(digit_expression_two.Coefficient * digit_expression_one.Coefficient,
                        digit_expression_two.LambdaDegree + digit_expression_one.LambdaDegree)))
            .SelectMany(digits => digits)
            .GroupBy(digits => digits.LambdaDegree)
            .Select(group =>
                new LambdaDigit(group.Aggregate(0d, (sum, current_digit) => sum += current_digit.Coefficient),
                    group.First().LambdaDegree))
            .ToList();
        return new LambdaExpression(new_digits);
    }

    public override string ToString()
    {
        return $"[{string.Concat(_digits)}]";
    }
}