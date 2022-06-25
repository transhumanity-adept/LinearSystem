# Solve Linear Systems
Решение систем линейных алгебраических уравнений методами
* Якоби
* Гаусса-Зейделя
* Релаксации

## Решение
![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/SolveExample.jpg)
## Статистика
![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/GenerateExample.jpg)
![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/TestCI.jpg)
![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/TestET.jpg)
![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/TestAR.jpg)
![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/TestER.jpg)
## Алгоритмы

### Якоби

Метод Якоби представляет собой алгоритм нахождения решения системы линейных алгебраических уравнений. 
При условии наличия начального решения формула поиска корней имеет вид:

![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/Jacobi.png)

### Гаусс-Зейдель

Следующим методов является метод Гаусса-Зейделя. 
Основная идея метода заключается в использовании уже вычисленных корней для вычисления следующего корня.
Позволяет использовать меньше памяти. При условии наличия начального решения формула поиска корней имеет вид:

![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/GaussSeidel.png)

### Релаксация

Метод релаксации является модификацией метода Зейделя. 
Для получения решения вводится параметр 𝜔 который определяет в каких долях будет использовано старое и новое вычисленное значение корня. 
При условии наличия начального решения формула поиска корней имеет вид:

![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/SOR.png)

Для нахождения оптимального значения параметра 𝜔 используется следующая формула:

![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/SORparam.png)

Где 𝑝 – спектральный радиус матрицы, вычисляемый по формуле:

![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/Spectral.png)

В этой формуле λ – собственные значения матрицы, которые могут быть получены в виде корней полинома, представленного в следующем виде:

![alter](https://github.com/transhumanity-adept/LinearSystem/blob/master/representationImages/Polynomial.png)
