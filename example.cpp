begin
if 1 {
    if 1 debug 1
    if 0 debug 2
}
if 0 {
    if 1 debug 3
    if 0 debug 4
}
end






{
    dim a
    set a 5
    debug a
}

debug [2[43][54[54[45]]]]



set i 0
while -[i 10] {
    set i +[i 1]
    debug i
}


set a 1
set b 0
while 1 {
    debug a
    set c +[a b]
    = a b
    = b c
}


def oneplus(a) [
    +[a 1]
]


debug oneplus[1]


def foreverdebug() {
    debug()
    foreverdebug()
}

foreverdebug()



def matrix() {
    dim n
    dim m
}



def bool(a) if a 1 else 0
def !!(a) if a 0 else 1
def &&(a b) if a if b 1 else 0 else 0
def ||(a b) [
    {
        = ret 0
        if a = ret 1
        if b = ret 1
    }
    ret
]
def ^^(a b) if a if b 0 else 1 else if b 1 else 0
def >=(a b) !! <[a b]
def ==(a b) &&(>=(a b) >=(b a))
def !=(a b) !! ==(a b)
def >(a b) &&(>=(a b) !=(a b))
def <=(a b) !! >(a b)


debug(bool(1))


def sum[a] 


@ 0 a
