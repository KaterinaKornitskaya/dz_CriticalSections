# dz_CriticalSections
Создайте приложение, использующее критические секции. 
Создайте несколько потоков: первый поток генерирует и сохраняет в файл некоторое кол-во пар чисел. 
Второй поток ожидает завершение генерации, после чего подсчитывает сумму каждой пары, результат записывается в файл. 
Третий поток также ожидает завершение генерации, после чего подсчитывает произведение кажой пары, результат записывает в файл.