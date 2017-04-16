EXECUTABLE := benchmark-farmhash
EXECUTABLE_NO_HA := benchmark-farmhash-no-ha
OBJS := farmhash.o benchmark-farmhash.o
OBJS_NO_HA := farmhash-no-ha.o benchmark-farmhash.o
CXXFLAGS := -std=c++11 -O3 -I../lib/benchmark/include -I../lib/farmhash/src
LINK_FLAGS := -L../lib/benchmark/src -lpthread -lbenchmark -pthread
BENCH_DIR := ../lib/benchmark
all: $(EXECUTABLE) $(EXECUTABLE_NO_HA)

$(EXECUTABLE): $(OBJS) $(BENCH_DIR)/src/libbenchmark.a
				g++ -g -o $@ $(OBJS) $(LINK_FLAGS) $(BENCH_DIR)/src/libbenchmark.a

$(EXECUTABLE_NO_HA): $(OBJS_NO_HA) $(BENCH_DIR)/src/libbenchmark.a
				g++ -g -o $@ $(OBJS_NO_HA) $(LINK_FLAGS) $(BENCH_DIR)/src/libbenchmark.a

benchmark-farmhash.o: benchmark-farmhash.cpp

farmhash.o: ../lib/farmhash/src/farmhash.cc ../lib/farmhash/src/farmhash.h
				g++ -c $(CXXFLAGS) -maes -mavx $<

farmhash-no-ha.o: ../lib/farmhash/src/farmhash.cc ../lib/farmhash/src/farmhash.h
				g++ -c -o $@ $(CXXFLAGS) -mno-sse2 $<

$(BENCH_DIR)/src/libbenchmark.a:
				cd $(BENCH_DIR) && \
                cmake -DCMAKE_BUILD_TYPE=Release -DBENCHMARK_ENABLE_LTO=true . && \
				make

clean:
				rm -f $(OBJS) $(OBJS_NO_HA) $(EXECUTABLE) $(EXECUTABLE_NO_HA)
