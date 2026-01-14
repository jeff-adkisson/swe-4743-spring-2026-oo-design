package inheritance.demo.main;

import inheritance.demo.entities.*;
import java.util.List;

public class Program {
    public static void main(String[] args) {
        Publication item1 = new PaperbackBook("A Philosophy of Software Design", 183);
        Publication item2 = new Scroll("War Scroll / Dead Sea Collection", 8148);

        List<Publication> myCollection = List.of(item1, item2);

        System.out.println("My Publication Collection [JAVA]:");
        for (Publication item : myCollection) {
            System.out.println("* " + item.getDetails());
        }
    }
}