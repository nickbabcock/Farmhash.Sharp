EXECUTABLE := benchmark-farmhash
EXECUTABLE_NO_HA := benchmark-farmhash-no-ha
OBJS := farmhash.o benchmark-farmhash.o
OBJS_NO_HA := farmhash-no-ha.o benchmark-farmhash.o
CXXFLAGS := -std=c++11 -O3 -I../lib/benchmark/include -I../lib/farmhash/src -march=native
LINK_FLAGS := -L../lib/benchmark/src -lpthread -lbenchmark -pthread
BENCH_DIR := ../lib/benchmark

all: $(EXECUTABLE) $(EXECUTABLE_NO_HA)

$(EXECUTABLE): $(OBJS) $(BENCH_DIR)/src/libbenchmark.a
				$(CXX) -g -o $@ $(OBJS) $(LINK_FLAGS) $(BENCH_DIR)/src/libbenchmark.a

$(EXECUTABLE_NO_HA): $(OBJS_NO_HA) $(BENCH_DIR)/src/libbenchmark.a
				$(CXX) -g -o $@ $(OBJS_NO_HA) $(LINK_FLAGS) $(BENCH_DIR)/src/libbenchmark.a

benchmark-farmhash.o: benchmark-farmhash.cpp

# From farmhash readme:
# To get the best performance from FarmHash, one will need to think a bit about
# when to use compiler flags that allow vector instructions and such: -maes,
# -msse4.2, -mavx
farmhash.o: ../lib/farmhash/src/farmhash.cc ../lib/farmhash/src/farmhash.h
				$(CXX) -c $(CXXFLAGS) -maes -msse4.2 -mavx $<

farmhash-no-ha.o: ../lib/farmhash/src/farmhash.cc ../lib/farmhash/src/farmhash.h
				$(CXX) -c -o $@ $(CXXFLAGS) -mno-avx -mno-aes -mno-sse2 $<

$(BENCH_DIR)/src/libbenchmark.a:
				cd $(BENCH_DIR) && \
				cmake -DCMAKE_BUILD_TYPE=Release -DBENCHMARK_ENABLE_GTEST_TESTS=OFF -DBENCHMARK_ENABLE_LTO=true . && \
				make

clean:
				make -C "../lib/benchmark" clean
				make -C "../lib/farmhash" clean
				rm -f $(OBJS) $(OBJS_NO_HA) $(EXECUTABLE) $(EXECUTABLE_NO_HA)
