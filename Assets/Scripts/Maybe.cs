using System;

public class Maybe<A> {
    private Maybe(){}

    public class None : Maybe<A> {
        internal None(){}
    }

    public class Some : Maybe<A> {
        private A _value;
        public A Value 
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException ();
                }
            }
        }
        internal Some(A value) {
            this._value = value;
        }
    }
    
    public static None NONE() {
        return new None();
    }

    public static Some SOME(A value) {
        return new Some(value);
    }
}